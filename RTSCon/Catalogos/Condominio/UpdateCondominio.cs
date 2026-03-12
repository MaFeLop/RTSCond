using Krypton.Toolkit;
using RTSCon.Catalogos;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class UpdateCondominio : KryptonForm
    {
        private readonly NCondominio _neg;
        private int _id;
        private byte[] _rowVersion;
        private int? _idPropietarioSel;

        public int CondominioId { get; set; }

        public UpdateCondominio()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cn))
                throw new InvalidOperationException("No se encontró la cadena de conexión 'RTSCond'.");

            _neg = new NCondominio(new DCondominio(cn));

            Shown += (s, e) =>
            {
                InitUi();

                if (!ResolverId())
                    return;

                CargarDatos();
            };

            if (btnConfirmar != null)
                btnConfirmar.Click += btnConfirmar_Click;

            if (btnVolver != null)
                btnVolver.Click += (s, e) => Close();

            if (btnBuscarPropietario != null)
                btnBuscarPropietario.Click += btnBuscarPropietario_Click;

            SetKeyPressNumeric(TxtCuota);
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

            if (TxtPropietario != null)
                TxtPropietario.ReadOnly = true;

            if (TxtIdPropietario != null)
                TxtIdPropietario.ReadOnly = true;
        }

        private bool ResolverId()
        {
            if (CondominioId > 0)
            {
                _id = CondominioId;
                return true;
            }

            if (Tag is int t && t > 0)
            {
                _id = t;
                return true;
            }

            if (Tag != null && int.TryParse(Convert.ToString(Tag), out int parsed) && parsed > 0)
            {
                _id = parsed;
                return true;
            }

            KryptonMessageBox.Show(
                this,
                "No se recibió el Id del condominio.",
                "Actualizar Condominio",
                KryptonMessageBoxButtons.OK,
                KryptonMessageBoxIcon.Error);

            Close();
            return false;
        }

        // =========================
        // Carga de datos
        // =========================

        private void CargarDatos()
        {
            DataTable dt = _neg.BuscarPorId(_id);

            if (dt == null || dt.Rows.Count == 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "Condominio no encontrado.",
                    "Actualizar Condominio",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);

                Close();
                return;
            }

            DataRow r = dt.Rows[0];

            string GS(string col)
            {
                return dt.Columns.Contains(col) && r[col] != DBNull.Value
                    ? Convert.ToString(r[col])
                    : string.Empty;
            }

            decimal GD(string col)
            {
                return dt.Columns.Contains(col) && r[col] != DBNull.Value
                    ? Convert.ToDecimal(r[col])
                    : 0m;
            }

            DateTime GDT(string col)
            {
                return dt.Columns.Contains(col) && r[col] != DBNull.Value
                    ? Convert.ToDateTime(r[col])
                    : DateTime.Today;
            }

            bool GB(string col)
            {
                return dt.Columns.Contains(col) &&
                       r[col] != DBNull.Value &&
                       Convert.ToBoolean(r[col]);
            }

            if (TxtNombre != null)
                TxtNombre.Text = GS("Nombre");

            if (TxtDireccion != null)
                TxtDireccion.Text = GS("Direccion");

            if (TxtPropietario != null)
                TxtPropietario.Text = GS("AdministradorResponsable");

            if (TxtCorreoNotif != null)
                TxtCorreoNotif.Text = GS("EmailNotificaciones");

            if (TxtCuota != null)
                TxtCuota.Text = GD("CuotaMantenimientoBase").ToString(CultureInfo.CurrentCulture);

            if (DtpFecha != null)
                DtpFecha.Value = GDT("FechaConstitucion");

            if (CboTipo != null)
            {
                string tipo = GS("Tipo");
                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    if (!CboTipo.Items.Contains(tipo))
                        CboTipo.Items.Add(tipo);

                    CboTipo.SelectedItem = tipo;
                }
            }

            if (ChkNotifProp != null)
                ChkNotifProp.Checked = GB("EnviarNotifsAlPropietario");

            _rowVersion = dt.Columns.Contains("RowVersion") && r["RowVersion"] != DBNull.Value
                ? (byte[])r["RowVersion"]
                : null;

            if (dt.Columns.Contains("ID_propietario") && r["ID_propietario"] != DBNull.Value)
            {
                _idPropietarioSel = Convert.ToInt32(r["ID_propietario"]);

                if (TxtIdPropietario != null)
                    TxtIdPropietario.Text = _idPropietarioSel.ToString();
            }
            else
            {
                _idPropietarioSel = null;

                if (TxtIdPropietario != null)
                    TxtIdPropietario.Text = string.Empty;
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

                    _idPropietarioSel = f.SelectedId;

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
                    "Actualizar Condominio",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        // =========================
        // Guardar cambios
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

                if (_idPropietarioSel == null || _idPropietarioSel <= 0)
                {
                    if (EsPropietarioActual())
                        _idPropietarioSel = UserContext.UsuarioAuthId;
                    else
                        throw new InvalidOperationException("Debe seleccionar un propietario válido.");
                }

                if (!decimal.TryParse(cuotaTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal cuota) || cuota < 0)
                    throw new InvalidOperationException("Cuota inválida.");

                if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                    throw new InvalidOperationException("Correo inválido.");

                _neg.Actualizar(
                    _id,
                    nombre,
                    direccion,
                    tipo,
                    adminResp,
                    fechaConst,
                    cuota,
                    correo,
                    enviarNotifProp,
                    null,
                    _idPropietarioSel,
                    null,
                    null,
                    null,
                    _rowVersion,
                    ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local"
                );

                KryptonMessageBox.Show(
                    this,
                    "Condominio actualizado correctamente.",
                    "Actualizar Condominio",
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
                    "Actualizar Condominio",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }
    }
}