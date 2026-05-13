using Krypton.Toolkit;
using RTSCon.Catalogos;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class CrearCondominio : KryptonForm
    {
        private readonly NCondominio _neg;
        private int? _propietarioId;
        private bool _uiInicializada;
        private bool _guardando;

        public CrearCondominio()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cn))
                throw new InvalidOperationException("No se encontró la cadena de conexión 'RTSCond'.");

            _neg = new NCondominio(new DCondominio(cn));

            ConfigurarEventos();
        }

        #region Propiedades visuales

        private KryptonTextBox TxtNombre => txtBuscar;
        private KryptonTextBox TxtDireccion => kryptonTextBox1;
        private KryptonTextBox TxtPropietario => kryptonTextBox2;
        private KryptonTextBox TxtIdPropietario => txtIdPropietario;
        private KryptonTextBox TxtCorreoNotif => txtCorreo;
        private KryptonTextBox TxtCuota => txtMantenimiento;
        private ComboBox CboTipo => cmbTipoCond;
        private KryptonDateTimePicker DtpFecha => dtpFechaConstitucion;
        private KryptonCheckBox ChkNotifProp => chkNotificarPropietario;

        #endregion

        #region Configuración inicial

        private void ConfigurarEventos()
        {
            this.Load -= CrearCondominio_Load;
            this.Load += CrearCondominio_Load;

            if (btnConfirmar != null)
            {
                btnConfirmar.Click -= btnConfirmar_Click;
                btnConfirmar.Click += btnConfirmar_Click;
            }

            if (btnVolver != null)
            {
                btnVolver.Click -= btnVolver_Click;
                btnVolver.Click += btnVolver_Click;
            }

            if (btnBuscarPropietario != null)
            {
                btnBuscarPropietario.Click -= btnBuscarPropietario_Click;
                btnBuscarPropietario.Click += btnBuscarPropietario_Click;
            }
        }

        private void InitUi()
        {
            if (_uiInicializada)
                return;

            _uiInicializada = true;

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
                    TxtIdPropietario.Text = string.Empty;

                if (btnBuscarPropietario != null)
                    btnBuscarPropietario.Enabled = true;
            }
        }

        private void SetKeyPressNumeric(KryptonTextBox tb)
        {
            if (tb == null)
                return;

            tb.KeyPress -= TxtCuota_KeyPress;
            tb.KeyPress += TxtCuota_KeyPress;
        }

        private void TxtCuota_KeyPress(object sender, KeyPressEventArgs e)
        {
            var tb = sender as KryptonTextBox;
            if (tb == null)
                return;

            char dec = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != dec)
            {
                e.Handled = true;
            }

            if (e.KeyChar == dec && tb.Text.Contains(dec.ToString()))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Validaciones auxiliares

        private bool EsPropietarioActual()
        {
            return !string.IsNullOrWhiteSpace(UserContext.Rol) &&
                   UserContext.Rol.Equals("Propietario", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Eventos principales

        private void CrearCondominio_Load(object sender, EventArgs e)
        {
            InitUi();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new BuscarPropietario(_neg))
                {
                    if (f.ShowDialog(this) != DialogResult.OK)
                        return;

                    _propietarioId = f.SelectedId;

                    if (TxtPropietario != null)
                        TxtPropietario.Text = f.SelectedUsuario ?? string.Empty;

                    if (TxtIdPropietario != null)
                        TxtIdPropietario.Text = f.SelectedDocumento ?? string.Empty;

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

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (_guardando)
                return;

            _guardando = true;

            if (btnConfirmar != null)
                btnConfirmar.Enabled = false;

            try
            {
                CrearRegistro();
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
            finally
            {
                if (!IsDisposed && btnConfirmar != null)
                    btnConfirmar.Enabled = true;

                _guardando = false;
            }
        }

        #endregion

        #region Lógica de creación

        private void CrearRegistro()
        {
            string nombre = TxtNombre?.Text?.Trim() ?? string.Empty;
            string direccion = TxtDireccion?.Text?.Trim() ?? string.Empty;
            string tipo = CboTipo?.SelectedItem?.ToString()?.Trim() ?? string.Empty;
            string adminResp = TxtPropietario?.Text?.Trim() ?? string.Empty;
            string correo = TxtCorreoNotif?.Text?.Trim() ?? string.Empty;
            string cuotaTexto = TxtCuota?.Text?.Trim() ?? string.Empty;
            DateTime fechaConst = DtpFecha?.Value.Date ?? DateTime.Today;
            bool enviarNotifProp = ChkNotifProp?.Checked ?? false;

            ValidarDatos(nombre, direccion, tipo, adminResp, correo, cuotaTexto);

            decimal cuotaBase = decimal.Parse(cuotaTexto, NumberStyles.Number, CultureInfo.CurrentCulture);

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
                "Condominio creado correctamente (Id " + nuevoId + ").",
                "Crear Condominio",
                KryptonMessageBoxButtons.OK,
                KryptonMessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ValidarDatos(
            string nombre,
            string direccion,
            string tipo,
            string adminResp,
            string correo,
            string cuotaTexto)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new InvalidOperationException("Ingrese el nombre del condominio.");

            if (string.IsNullOrWhiteSpace(direccion))
                throw new InvalidOperationException("Ingrese la dirección del condominio.");

            if (string.IsNullOrWhiteSpace(tipo))
                throw new InvalidOperationException("Seleccione el tipo de condominio.");

            if (string.IsNullOrWhiteSpace(adminResp))
                throw new InvalidOperationException("Seleccione el propietario responsable.");

            if (_propietarioId == null || _propietarioId <= 0)
                throw new InvalidOperationException("Debe seleccionar un propietario válido.");

            decimal cuotaBase;
            if (!decimal.TryParse(cuotaTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out cuotaBase) || cuotaBase < 0)
                throw new InvalidOperationException("Cuota de mantenimiento inválida.");

            if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                throw new InvalidOperationException("Correo de notificaciones inválido.");
        }

        #endregion

        #region Eventos vacíos del Designer

        private void txtAdministrador_Click(object sender, EventArgs e) { }
        private void kryptonTextBox2_TextChanged(object sender, EventArgs e) { }
        private void dtpFechaConstitucion_ValueChanged(object sender, EventArgs e) { }
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

        #endregion
    }
}