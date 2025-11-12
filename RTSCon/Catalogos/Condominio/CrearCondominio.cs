using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class CrearCondominio : KryptonForm
    {
        private readonly NCondominio _neg;

        // Límites de longitud según tu BD (ajusta si cambian en el esquema)
        private const int MAX_NOMBRE = 200;
        private const int MAX_DIR = 300;
        private const int MAX_TIPO = 30;
        private const int MAX_ADMIN = 120;  // asunción razonable
        private const int MAX_EMAIL = 150;
        public CrearCondominio()
        {
            InitializeComponent();
            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            // Inicialización UI
            this.Shown += (s, e) => InitUi();

            // Botones
            var btnOk = FindCtrl<KryptonButton>("btnConfirmar", "btnGuardar", "btnOk");
            if (btnOk != null) btnOk.Click += btnConfirmar_Click;

            var btnBack = FindCtrl<KryptonButton>("btnVolver", "btnCancelar", "btnBack");
            if (btnBack != null) btnBack.Click += (s, e) => this.Close();
        }

        // ---------- Helpers ----------
        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (var n in names)
            {
                var c = this.Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null) return c;
            }
            return null;
        }
        private string GetText(params string[] names)
        {
            var c = FindCtrl<Control>(names);
            return c?.Text?.Trim() ?? "";
        }
        private static string Trunc(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length > max ? s.Substring(0, max) : s;
        }
        private void SetKeyPressNumeric(params string[] names)
        {
            var c = FindCtrl<Control>(names);
            if (c == null) return;
            c.KeyPress += (s, e) =>
            {
                char dec = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != dec)
                    e.Handled = true;

                // .NET 4.8: Contains(char) no existe -> usar string
                if (e.KeyChar == dec && (s as Control).Text.Contains(dec.ToString()))
                    e.Handled = true;
            };
        }
        // -----------------------------

        private void InitUi()
        {
            // Tipo de condominio
            var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
            if (cboTipo != null && cboTipo.Items.Count == 0)
            {
                cboTipo.Items.AddRange(new object[] { "Residencial", "Comercial", "Mixto" });
                cboTipo.SelectedIndex = 0;
            }

            // Fecha hoy por defecto
            var dtp = FindCtrl<DateTimePicker>("dtpFechaConstitucion", "dtpFechaConst");
            if (dtp != null && dtp.Value == default(DateTime)) dtp.Value = DateTime.Today;

            // Solo números en cuota
            SetKeyPressNumeric("txtCuotaBase", "txtMantenimientoBase", "txtCuota");
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void kryptonTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // Controles (con alias)
                var txtNombre = GetText("txtNombreCondominio", "txtNombre");
                var txtDireccion = GetText("txtDireccionCondominio", "txtDireccion");
                var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
                var txtAdminResp = GetText("txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                var txtCorreo = GetText("txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
                var txtCuota = GetText("txtCuotaBase", "txtMantenimientoBase", "txtCuota");
                var dtp = FindCtrl<DateTimePicker>("dtpFechaConstitucion", "dtpFechaConst");
                var chkNotifProp = FindCtrl<KryptonCheckBox>("chkNotificarPropietario", "chkEnviarNotifsPropietario", "chkEnviarNotificaciones");

                // Validaciones requeridas (NOT NULL en BD)
                if (string.IsNullOrWhiteSpace(txtNombre))
                    throw new InvalidOperationException("Ingrese el nombre del condominio.");
                if (string.IsNullOrWhiteSpace(txtDireccion))
                    throw new InvalidOperationException("Ingrese la dirección del condominio.");
                if (cboTipo == null || string.IsNullOrWhiteSpace(Convert.ToString(cboTipo.SelectedItem)))
                    throw new InvalidOperationException("Seleccione el tipo de condominio.");
                if (string.IsNullOrWhiteSpace(txtAdminResp))
                    throw new InvalidOperationException("Ingrese el propietario/administrador responsable.");
                if (dtp == null)
                    throw new InvalidOperationException("Seleccione la fecha de constitución.");
                if (!decimal.TryParse(txtCuota, NumberStyles.Number, CultureInfo.CurrentCulture, out var cuota) || cuota < 0)
                    throw new InvalidOperationException("Cuota de mantenimiento inválida.");
                if (string.IsNullOrWhiteSpace(txtCorreo) || !txtCorreo.Contains("@"))
                    throw new InvalidOperationException("Correo de notificaciones inválido.");

                // Recorte por longitud (para evitar errores por tamaño en NVARCHAR)
                string nombre = Trunc(txtNombre, MAX_NOMBRE);
                string direccion = Trunc(txtDireccion, MAX_DIR);
                string tipo = Trunc(Convert.ToString(cboTipo.SelectedItem), MAX_TIPO);
                string adminResp = Trunc(txtAdminResp, MAX_ADMIN);
                string emailNotif = Trunc(txtCorreo, MAX_EMAIL);

                DateTime fechaConstitucion = dtp.Value.Date;
                decimal cuotaBase = cuota;
                bool enviarNotifProp = chkNotifProp?.Checked ?? false;

                // FKs opcionales, por ahora NULL
                int? reglamentoDocId = null;
                int? idPropietario = null;
                int? idSec1 = null, idSec2 = null, idSec3 = null;

                string creador = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                // Insertar
                int nuevoId = _neg.Insertar(
                    nombre, direccion, tipo, adminResp,
                    fechaConstitucion, cuotaBase,
                    emailNotif, enviarNotifProp,
                    reglamentoDocId, idPropietario, idSec1, idSec2, idSec3,
                    creador);

                KryptonMessageBox.Show(this, $"Condominio creado (Id {nuevoId}).",
                    "Crear Condominio", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message,
                    "Crear Condominio", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbTipoCond_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }

        private void kryptonTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void kryptonTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dtpFechaConstitucion_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
