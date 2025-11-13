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

        public CrearCondominio()
        {
            InitializeComponent();
            var btnBuscarProp = this.Controls.Find("btnBuscarPropietario", true)
                                 .OfType<Krypton.Toolkit.KryptonButton>()
                                 .FirstOrDefault();

            if (btnBuscarProp != null)
                btnBuscarProp.Click += btnBuscarPropietario_Click;

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            this.Shown += (_, __) => InitUi();
            btnConfirmar.Click += btnConfirmar_Click;
            btnVolver.Click += (_, __) => Close();

            // Si más adelante agregas un botón buscar propietario (btnBuscarPropietario), lo conectamos:
            if (btnBuscarProp != null) btnBuscarProp.Click += (_, __) => AbrirBuscarPropietario();
        }

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
        private void SetKeyPressNumeric(params string[] names)
        {
            var c = FindCtrl<Control>(names);
            if (c == null) return;
            c.KeyPress += (s, e) =>
            {
                char dec = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != dec) e.Handled = true;
                if (e.KeyChar == dec && (s as Control).Text.Contains(dec)) e.Handled = true;
            };
        }

        private void InitUi()
        {
            // Tipo
            var cboTipo = cmbTipoCond; // ya existe en tu Designer
            if (cboTipo != null && cboTipo.Items.Count == 0)
            {
                cboTipo.Items.AddRange(new object[] { "Residencial", "Comercial", "Mixto" });
                cboTipo.SelectedIndex = 0;
            }

            // Fecha: si tienes un DateTimePicker con otro nombre, cámbialo aquí
            var dtp = this.Controls.OfType<DateTimePicker>().FirstOrDefault();
            if (dtp != null && dtp.Value == default(DateTime)) dtp.Value = DateTime.Today;

            // Solo números en cuota base (en tu Designer es kryptonTextBox4)
            SetKeyPressNumeric("kryptonTextBox4");
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // Tus nombres reales del Designer:
                string nombre = GetText("txtBuscar");                 // Nombre del condominio (!)
                string direccion = GetText("kryptonTextBox1");        // Dirección
                string tipo = cmbTipoCond?.SelectedItem?.ToString();  // Tipo
                string adminResp = GetText("kryptonTextBox3");        // Propietario/Administrador responsable (de momento texto)
                string identificacionProp = GetText("kryptonTextBox2"); // Identificación del propietario
                string emailNotif = GetText("kryptonTextBox6");       // Correo Notificaciones
                string cuotaTxt = GetText("kryptonTextBox4");         // Cuota Base

                var dtp = this.Controls.OfType<DateTimePicker>().FirstOrDefault();
                DateTime fechaConstitucion = dtp?.Value.Date ?? DateTime.Today;

                // Validaciones mínimas
                if (string.IsNullOrWhiteSpace(nombre)) throw new InvalidOperationException("Ingrese el nombre del condominio.");
                if (string.IsNullOrWhiteSpace(direccion)) throw new InvalidOperationException("Ingrese la dirección.");
                if (string.IsNullOrWhiteSpace(tipo)) throw new InvalidOperationException("Seleccione el tipo.");
                if (string.IsNullOrWhiteSpace(adminResp)) throw new InvalidOperationException("Ingrese el propietario responsable.");
                if (!decimal.TryParse(cuotaTxt, NumberStyles.Number, CultureInfo.CurrentCulture, out var cuota) || cuota < 0)
                    throw new InvalidOperationException("Cuota de mantenimiento inválida.");
                if (string.IsNullOrWhiteSpace(emailNotif) || !emailNotif.Contains("@"))
                    throw new InvalidOperationException("Correo de notificaciones inválido.");

                // Flags opcionales
                bool enviarNotifProp = (this.Controls.Find("chkEnviarNotificaciones", true).FirstOrDefault() as KryptonCheckBox)?.Checked ?? false;

                // (cuando tengas buscador real, mapearás ID_propietario; por ahora NULL)
                int? idPropietario = null;
                int? idSec1 = null, idSec2 = null, idSec3 = null; // todavía no enlazados a usuarios
                int? reglamentoDocId = null;

                string creador = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                // Insert
                int nuevoId = _neg.Insertar(
                    nombre, direccion, tipo, adminResp,
                    fechaConstitucion, cuota,
                    emailNotif, enviarNotifProp,
                    reglamentoDocId, idPropietario, idSec1, idSec2, idSec3,
                    creador);

                KryptonMessageBox.Show(this, $"Condominio creado (Id {nuevoId}).", "Crear Condominio",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message, "Crear Condominio",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Preparado para cuando agregues el formulario de búsqueda de propietarios.
        /// Si existe (BuscarPropietario), lo usa; si no, simplemente no hace nada.
        /// </summary>
        private void AbrirBuscarPropietario()
        {
            var t = Type.GetType("RTSCon.Catalogos.Condominio.BuscarPropietario");
            if (t == null) { KryptonMessageBox.Show(this, "El buscador aún no está disponible."); return; }

            using (var f = Activator.CreateInstance(t) as Form)
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    // El buscador debe exponer propiedades públicas con los datos seleccionados:
                    string nombre = (string)t.GetProperty("PropietarioNombre")?.GetValue(f);
                    string correo = (string)t.GetProperty("PropietarioCorreo")?.GetValue(f);
                    string doc = (string)t.GetProperty("PropietarioDocumento")?.GetValue(f);

                    var txtAdmin = this.Controls.Find("kryptonTextBox3", true).FirstOrDefault() as TextBoxBase;
                    var txtCorreo = this.Controls.Find("kryptonTextBox6", true).FirstOrDefault() as TextBoxBase;
                    var txtDoc = this.Controls.Find("kryptonTextBox2", true).FirstOrDefault() as TextBoxBase;

                    if (txtAdmin != null) txtAdmin.Text = nombre ?? txtAdmin.Text;
                    if (txtCorreo != null) txtCorreo.Text = correo ?? txtCorreo.Text;
                    if (txtDoc != null) txtDoc.Text = doc ?? txtDoc.Text;
                }
            }
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            using (var f = new RTSCon.Catalogos.Condominio.BuscarPropietario())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    // Controles destino (usa los nombres reales que ya tienes en Create/Update)
                    var txtAdmin = this.Controls.Find("txtPropietarioResponsable", true).FirstOrDefault() as Control;
                    var txtCorreo = (this.Controls.Find("txtCorreoNotif", true).FirstOrDefault()
                                  ?? this.Controls.Find("txtCorreoNotificaciones", true).FirstOrDefault()
                                  ?? this.Controls.Find("txtCorreo", true).FirstOrDefault()) as Control;

                    // Setea nombre y guarda el Id en Tag
                    if (txtAdmin != null)
                    {
                        txtAdmin.Text = f.SelectedUsuario;   // nombre/usuario del propietario
                        txtAdmin.Tag = f.SelectedId;        // ID del propietario (para guardar en BD)
                    }

                    // Rellena correo si viene
                    if (txtCorreo != null && !string.IsNullOrWhiteSpace(f.SelectedCorreo))
                        txtCorreo.Text = f.SelectedCorreo;
                }
            }
        }
    }
}
