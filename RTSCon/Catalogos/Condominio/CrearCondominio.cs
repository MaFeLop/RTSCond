using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class CrearCondominio : KryptonForm
    {
        private readonly NCondominio _neg;
        private int? _propietarioId; // se envía al SP como @ID_propietario

        public CrearCondominio()
        {
            InitializeComponent();
            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            this.Shown += (s, e) => InitUi();

            var btnOk = FindCtrl<KryptonButton>("btnConfirmar", "btnGuardar", "btnOk");
            var btnBack = FindCtrl<KryptonButton>("btnVolver", "btnCancelar", "btnBack");
            if (btnOk != null) btnOk.Click += btnConfirmar_Click;
            if (btnBack != null) btnBack.Click += (s, e) => Close();

            // hookup del botón de búsqueda (si existe)
            var btnBuscarProp = FindCtrl<KryptonButton>("btnBuscarPropietario", "btnSelPropietario");
            if (btnBuscarProp != null) btnBuscarProp.Click += btnBuscarPropietario_Click;
        }

        // ---------- helpers ----------
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
        private void SetText(string value, params string[] names)
        {
            var c = FindCtrl<Control>(names);
            if (c != null) c.Text = value ?? "";
        }
        private void SetKeyPressNumeric(params string[] names)
        {
            var c = FindCtrl<TextBoxBase>(names);
            if (c == null) return;
            c.KeyPress += (s, e) =>
            {
                char dec = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != dec)
                    e.Handled = true;
                if (e.KeyChar == dec && ((TextBoxBase)s).Text.Contains(dec))
                    e.Handled = true;
            };
        }
        // -----------------------------

        private void InitUi()
        {
            // tipo
            var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
            if (cboTipo != null && cboTipo.Items.Count == 0)
            {
                cboTipo.Items.AddRange(new object[] { "Residencial", "Comercial", "Mixto" });
                cboTipo.SelectedIndex = 0;
            }

            // fecha hoy
            var dtp = FindCtrl<DateTimePicker>("dtpFechaConstitucion", "dtpFechaConst");
            if (dtp != null) dtp.Value = DateTime.Today;

            // numérico en cuota
            SetKeyPressNumeric("txtCuotaBase", "txtMantenimientoBase", "txtCuota");

            // ===== Admin = Propietario =====
            // Si el usuario actual es Admin o Propietario, precargamos su info
            if (UserContext.EsPropietarioActual)
            {
                _propietarioId = UserContext.UsuarioAuthId;
                SetText(UserContext.Usuario, "txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");

                var btnBuscarProp = FindCtrl<KryptonButton>("btnBuscarPropietario", "btnSelPropietario");
                if (btnBuscarProp != null) btnBuscarProp.Enabled = true; // permitir cambio si lo desea
            }
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            using (var f = new BuscarPropietario())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    _propietarioId = f.SelectedId;
                    SetText(f.SelectedUsuario, "txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                    // si quieres, también muestra el correo en un label/readonly
                    SetText(f.SelectedCorreo, "txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
                }
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                var txtNombre = GetText("txtNombreCondominio", "txtNombre");
                var txtDireccion = GetText("txtDireccionCondominio", "txtDireccion");
                var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
                var txtAdmin = GetText("txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                var txtCorreo = GetText("txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
                var txtCuota = GetText("txtCuotaBase", "txtMantenimientoBase", "txtCuota");
                var dtp = FindCtrl<DateTimePicker>("dtpFechaConstitucion", "dtpFechaConst");
                var chkNotifProp = FindCtrl<KryptonCheckBox>("chkNotificarPropietario", "chkEnviarNotifsPropietario", "chkEnviarNotificaciones");

                if (string.IsNullOrWhiteSpace(txtNombre)) throw new InvalidOperationException("Ingrese el nombre del condominio.");
                if (string.IsNullOrWhiteSpace(txtDireccion)) throw new InvalidOperationException("Ingrese la dirección del condominio.");
                if (cboTipo == null || string.IsNullOrWhiteSpace(Convert.ToString(cboTipo.SelectedItem)))
                    throw new InvalidOperationException("Seleccione el tipo de condominio.");
                if (string.IsNullOrWhiteSpace(txtAdmin)) throw new InvalidOperationException("Ingrese el propietario responsable.");
                if (dtp == null) throw new InvalidOperationException("Seleccione la fecha de constitución.");
                if (!decimal.TryParse(txtCuota, NumberStyles.Number, CultureInfo.CurrentCulture, out var cuota) || cuota < 0)
                    throw new InvalidOperationException("Cuota de mantenimiento inválida.");
                if (string.IsNullOrWhiteSpace(txtCorreo) || !txtCorreo.Contains("@"))
                    throw new InvalidOperationException("Correo de notificaciones inválido.");

                // ===== idPropietario al SP =====
                // Si no se eligió manualmente y el usuario actual es Admin/Propietario, usamos su Id.
                if (_propietarioId == null && UserContext.EsPropietarioActual)
                    _propietarioId = UserContext.UsuarioAuthId;

                string nombre = txtNombre;
                string direccion = txtDireccion;
                string tipo = Convert.ToString(cboTipo.SelectedItem);
                string adminResp = txtAdmin;
                DateTime fechaConst = dtp.Value.Date;
                decimal cuotaBase = cuota;
                string emailNotif = txtCorreo;
                bool enviarNotifProp = chkNotifProp?.Checked ?? false;

                int? reglamentoDocId = null;
                int? idPropietario = _propietarioId;
                int? idSec1 = null, idSec2 = null, idSec3 = null;

                string creador = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                int nuevoId = _neg.Insertar(
                    nombre, direccion, tipo, adminResp,
                    fechaConst, cuotaBase,
                    emailNotif, enviarNotifProp,
                    reglamentoDocId, idPropietario, idSec1, idSec2, idSec3,
                    creador);

                KryptonMessageBox.Show(this, $"Condominio creado (Id {nuevoId}).",
                    "Crear Condominio", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message,
                    "Crear Condominio", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }

        private void CrearCondominio_Load(object sender, EventArgs e)
        {

        }

        private void txtAdministrador_Click(object sender, EventArgs e)
        {

        }

        private void kryptonTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dtpFechaConstitucion_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnVolver_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtSecretario3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void txtSecretario2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void cmbTipoCond_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtIdPropietario_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSecretario1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkNotificarPropietario_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void txtCorreo_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void txtMantenimiento_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void kryptonTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }

        private void kryptonLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
