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

            SetKeyPressNumeric(txtMantenimiento);
        }

        private void SetKeyPressNumeric(KryptonTextBox tb)
        {
            if (tb == null)
                return;

            tb.KeyPress += (s, e) =>
            {
                string separadorDecimal = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                char dec = separadorDecimal[0];

                if (!char.IsControl(e.KeyChar) &&
                    !char.IsDigit(e.KeyChar) &&
                    e.KeyChar != dec)
                {
                    e.Handled = true;
                }

                if (e.KeyChar == dec && tb.Text.Contains(separadorDecimal))
                {
                    e.Handled = true;
                }
            };
        }

        private void InitUi()
        {
            if (cmbTipoCond != null)
            {
                cmbTipoCond.Items.Clear();
                cmbTipoCond.Items.AddRange(new object[]
                {
                    "Casas",
                    "Apartamentos",
                    "Apartaestudios",
                    "Penthouses"
                });

                if (cmbTipoCond.SelectedIndex < 0 && cmbTipoCond.Items.Count > 0)
                    cmbTipoCond.SelectedIndex = 0;
            }

            if (kryptonTextBox2 != null)
                kryptonTextBox2.ReadOnly = true;

            if (txtIdPropietario != null)
                txtIdPropietario.ReadOnly = true;
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

            txtBuscar.Text = GS("Nombre");
            kryptonTextBox1.Text = GS("Direccion");
            kryptonTextBox2.Text = GS("AdministradorResponsable");
            txtCorreo.Text = GS("EmailNotificaciones");
            txtMantenimiento.Text = GD("CuotaMantenimientoBase").ToString(CultureInfo.CurrentCulture);
            dtpFechaConstitucion.Value = GDT("FechaConstitucion");

            string tipo = GS("Tipo");
            if (!string.IsNullOrWhiteSpace(tipo))
            {
                if (!cmbTipoCond.Items.Contains(tipo))
                    cmbTipoCond.Items.Add(tipo);

                cmbTipoCond.SelectedItem = tipo;
            }

            chkNotificarPropietario.Checked = GB("EnviarNotifsAlPropietario");

            _rowVersion = dt.Columns.Contains("RowVersion") && r["RowVersion"] != DBNull.Value
                ? (byte[])r["RowVersion"]
                : null;

            if (dt.Columns.Contains("ID_propietario") && r["ID_propietario"] != DBNull.Value)
            {
                _idPropietarioSel = Convert.ToInt32(r["ID_propietario"]);

                if (dt.Columns.Contains("PropietarioDocumento") && r["PropietarioDocumento"] != DBNull.Value)
                    txtIdPropietario.Text = Convert.ToString(r["PropietarioDocumento"]);
                else
                    txtIdPropietario.Text = string.Empty;
            }
            else
            {
                _idPropietarioSel = null;
                txtIdPropietario.Text = string.Empty;
            }
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new RTSCon.Catalogos.BuscarPropietario())
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;

                    _idPropietarioSel = dlg.SelectedId;
                    kryptonTextBox2.Text = dlg.SelectedUsuario;
                    txtIdPropietario.Text = dlg.SelectedDocumento;

                    if (string.IsNullOrWhiteSpace(txtCorreo.Text) && !string.IsNullOrWhiteSpace(dlg.SelectedCorreo))
                        txtCorreo.Text = dlg.SelectedCorreo;
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

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtBuscar?.Text?.Trim() ?? string.Empty;
                string direccion = kryptonTextBox1?.Text?.Trim() ?? string.Empty;
                string tipo = cmbTipoCond?.SelectedItem?.ToString()?.Trim() ?? string.Empty;
                string adminResp = kryptonTextBox2?.Text?.Trim() ?? string.Empty;
                string correo = txtCorreo?.Text?.Trim() ?? string.Empty;
                string cuotaTexto = txtMantenimiento?.Text?.Trim() ?? string.Empty;
                DateTime fechaConst = dtpFechaConstitucion?.Value.Date ?? DateTime.Today;
                bool enviarNotifProp = chkNotificarPropietario?.Checked ?? false;

                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("Ingrese el nombre del condominio.");

                if (string.IsNullOrWhiteSpace(direccion))
                    throw new InvalidOperationException("Ingrese la dirección del condominio.");

                if (string.IsNullOrWhiteSpace(tipo))
                    throw new InvalidOperationException("Seleccione el tipo de condominio.");

                if (string.IsNullOrWhiteSpace(adminResp))
                    throw new InvalidOperationException("Seleccione el propietario responsable.");

                if (_idPropietarioSel == null || _idPropietarioSel <= 0)
                    throw new InvalidOperationException("Debe seleccionar un propietario válido.");

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