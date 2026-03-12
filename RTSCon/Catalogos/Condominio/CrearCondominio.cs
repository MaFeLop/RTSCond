using Krypton.Toolkit;
using RTSCon.Catalogos;
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
        private int? _propietarioId;

        public CrearCondominio()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cn))
                throw new InvalidOperationException("No se encontró la cadena de conexión 'RTSCond'.");

            _neg = new NCondominio(new DCondominio(cn));

            Shown += (s, e) => InitUi();

            if (btnConfirmar != null)
                btnConfirmar.Click += btnConfirmar_Click;

            if (btnVolver != null)
                btnVolver.Click += (s, e) => Close();

            if (btnBuscarPropietario != null)
                btnBuscarPropietario.Click += btnBuscarPropietario_Click;
        }

        // =========================
        // Helpers de controles reales
        // =========================

        private KryptonTextBox TxtNombre => txtBuscar;
        private KryptonTextBox TxtDireccion => kryptonTextBox1;
        private KryptonTextBox TxtPropietario => kryptonTextBox2;
        private KryptonTextBox TxtIdPropietario => txtIdPropietario;
        private KryptonTextBox TxtCorreoNotif => txtCorreo;
        private KryptonTextBox TxtCuota => txtMantenimiento;
        private ComboBox CboTipo => cmbTipoCond;
        private DateTimePicker DtpFecha => dtpFechaConstitucion;
        private KryptonCheckBox ChkNotifProp => chkNotificarPropietario;

        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (var n in names)
            {
                var c = Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null) return c;
            }
            return null;
        }

        private void SetKeyPressNumeric(TextBoxBase tb)
        {
            if (tb == null) return;

            tb.KeyPress += (s, e) =>
            {
                char dec = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                if (!char.IsControl(e.KeyChar) &&
                    !char.IsDigit(e.KeyChar) &&
                    e.KeyChar != dec)
                {
                    e.Handled = true;
                }

                if (e.KeyChar == dec && tb.Text.Contains(dec))
                {
                    e.Handled = true;
                }
            };
        }

        private bool EsPropietarioActual()
        {
            return !string.IsNullOrWhiteSpace(UserContext.Rol) &&
                   UserContext.Rol.Equals("Propietario", StringComparison.OrdinalIgnoreCase);
        }

        // =========================
        // Inicialización
        // =========================

        private void InitUi()
        {
            if (CboTipo != null && CboTipo.Items.Count == 0)
            {
                CboTipo.Items.AddRange(new object[]
                {
                    "Residencial",
                    "Comercial",
                    "Mixto"
                });
            }

            if (CboTipo != null && CboTipo.SelectedIndex < 0 && CboTipo.Items.Count > 0)
                CboTipo.SelectedIndex = 0;

            if (DtpFecha != null)
                DtpFecha.Value = DateTime.Today;

            SetKeyPressNumeric(TxtCuota);

            if (TxtPropietario != null)
                TxtPropietario.ReadOnly = true;

            if (TxtIdPropietario != null)
                TxtIdPropietario.ReadOnly = true;

            if (EsPropietarioActual())
            {
                _propietarioId = UserContext.UsuarioAuthId;

                if (TxtPropietario != null)
                    TxtPropietario.Text = UserContext.Usuario ?? string.Empty;

                if (TxtIdPropietario != null)
                    TxtIdPropietario.Text = UserContext.UsuarioAuthId.ToString();

                if (btnBuscarPropietario != null)
                    btnBuscarPropietario.Enabled = true;
            }
        }

        // =========================
        // Buscar propietario
        // =========================

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new BuscarPropietario())
                {
                    if (f.ShowDialog(this) != DialogResult.OK)
                        return;

                    _propietarioId = f.SelectedId;

                    if (TxtPropietario != null)
                        TxtPropietario.Text = f.SelectedUsuario ?? string.Empty;

                    if (TxtIdPropietario != null)
                        TxtIdPropietario.Text = f.SelectedId > 0 ? f.SelectedId.ToString() : string.Empty;

                    if (TxtCorreoNotif != null &&
                        string.IsNullOrWhiteSpace(TxtCorreoNotif.Text) &&
                        !string.IsNullOrWhiteSpace(f.SelectedCorreo))
                    {
                        TxtCorreoNotif.Text = f.SelectedCorreo;
                    }
                }
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "Error al seleccionar propietario: " + ex.Message,
                    "Crear Condominio",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        // =========================
        // Guardar
        // =========================

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = TxtNombre?.Text?.Trim() ?? string.Empty;
                string direccion = TxtDireccion?.Text?.Trim() ?? string.Empty;
                string tipo = CboTipo?.SelectedItem?.ToString()?.Trim() ?? string.Empty;
                string adminResp = TxtPropietario?.Text?.Trim() ?? string.Empty;
                string correo = TxtCorreoNotif?.Text?.Trim() ?? string.Empty;
                string cuotaTexto = TxtCuota?.Text?.Trim() ?? string.Empty;
                DateTime fechaConst = DtpFecha?.Value.Date ?? DateTime.Today;
                bool enviarNotifProp = ChkNotifProp?.Checked ?? false;

                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("Ingrese el nombre del condominio.");

                if (string.IsNullOrWhiteSpace(direccion))
                    throw new InvalidOperationException("Ingrese la dirección del condominio.");

                if (string.IsNullOrWhiteSpace(tipo))
                    throw new InvalidOperationException("Seleccione el tipo de condominio.");

                if (string.IsNullOrWhiteSpace(adminResp))
                    throw new InvalidOperationException("Seleccione el propietario responsable.");

                if (_propietarioId == null || _propietarioId <= 0)
                {
                    if (EsPropietarioActual())
                        _propietarioId = UserContext.UsuarioAuthId;
                    else
                        throw new InvalidOperationException("Debe seleccionar un propietario válido.");
                }

                if (!decimal.TryParse(cuotaTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal cuotaBase) || cuotaBase < 0)
                    throw new InvalidOperationException("Cuota de mantenimiento inválida.");

                if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                    throw new InvalidOperationException("Correo de notificaciones inválido.");

                int? reglamentoDocId = null;
                int? idPropietario = _propietarioId;
                int? idSec1 = null;
                int? idSec2 = null;
                int? idSec3 = null;

                string creador = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                int nuevoId = _neg.Insertar(
                    nombre,
                    direccion,
                    tipo,
                    adminResp,
                    fechaConst,
                    cuotaBase,
                    correo,
                    enviarNotifProp,
                    reglamentoDocId,
                    idPropietario,
                    idSec1,
                    idSec2,
                    idSec3,
                    creador
                );

                KryptonMessageBox.Show(
                    this,
                    $"Condominio creado correctamente (Id {nuevoId}).",
                    "Crear Condominio",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    ex.Message,
                    "Crear Condominio",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        // =========================
        // Eventos vacíos del diseñador
        // =========================

        private void CrearCondominio_Load(object sender, EventArgs e) { }
        private void txtAdministrador_Click(object sender, EventArgs e) { }
        private void kryptonTextBox2_TextChanged(object sender, EventArgs e) { }
        private void dtpFechaConstitucion_ValueChanged(object sender, EventArgs e) { }
        private void btnVolver_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void txtSecretario3_TextChanged(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void txtSecretario2_TextChanged(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void cmbTipoCond_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void txtIdPropietario_TextChanged(object sender, EventArgs e) { }
        private void txtSecretario1_TextChanged(object sender, EventArgs e) { }
        private void chkNotificarPropietario_CheckedChanged(object sender, EventArgs e) { }
        private void label13_Click(object sender, EventArgs e) { }
        private void txtCorreo_TextChanged(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void txtMantenimiento_TextChanged(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void kryptonTextBox1_TextChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void txtBuscar_TextChanged(object sender, EventArgs e) { }
        private void kryptonLabel1_Click(object sender, EventArgs e) { }
    }
}