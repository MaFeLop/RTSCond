using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class UpdatePropiedad : Form
    {
        private readonly NPropiedad _neg;
        private int _id;
        private byte[] _rowVersion;
        private int? _propietarioIdSeleccionado;

        public UpdatePropiedad()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            Shown += (_, __) => InitUiAndLoad();

            if (btnConfirmar != null)
                btnConfirmar.Click += btnConfirmar_Click;

            if (btnVolver != null)
                btnVolver.Click += (_, __) => Close();

            if (btnBuscarPropietario != null)
                btnBuscarPropietario.Click += btnBuscarPropietario_Click;

            SetKeyPressDecimal(txtPorcentaje);

            if (txtIdUnidad != null)
            {
                txtIdUnidad.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        btnConfirmar?.PerformClick();
                    }
                };
            }

            if (txtPorcentaje != null)
            {
                txtPorcentaje.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        btnConfirmar?.PerformClick();
                    }
                };
            }
        }

        private void SetKeyPressDecimal(KryptonTextBox tb)
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

        private void InitUiAndLoad()
        {
            if (Tag == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se recibió el Id de la propiedad.",
                    "Actualizar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);

                Close();
                return;
            }

            try
            {
                if (Tag is int i)
                    _id = i;
                else if (!int.TryParse(Tag.ToString(), out _id))
                    throw new InvalidOperationException("Id de propiedad inválido.");
            }
            catch
            {
                KryptonMessageBox.Show(
                    this,
                    "Id de propiedad inválido.",
                    "Actualizar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);

                Close();
                return;
            }

            if (dtpFechaInicio != null)
                dtpFechaInicio.Value = DateTime.Today;

            if (dtpFechaFin != null)
                dtpFechaFin.Value = DateTime.Today;

            if (txtNombrePropiedad != null)
                txtNombrePropiedad.ReadOnly = false;

            if (txtNombrePropietario != null)
                txtNombrePropietario.ReadOnly = true;

            if (txtPropietarioDocumento != null)
                txtPropietarioDocumento.ReadOnly = true;

            CargarDatos();
        }

        private void CargarDatos()
        {
            DataRow row = _neg.PorId(_id);
            if (row == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se encontró la propiedad.",
                    "Actualizar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);

                Close();
                return;
            }

            if (row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                _rowVersion = (byte[])row["RowVersion"];

            if (row.Table.Columns.Contains("Nombre") && row["Nombre"] != DBNull.Value)
                txtNombrePropiedad.Text = Convert.ToString(row["Nombre"]);

            if (row.Table.Columns.Contains("UnidadId") && row["UnidadId"] != DBNull.Value)
                txtIdUnidad.Text = Convert.ToString(row["UnidadId"]);

            if (row.Table.Columns.Contains("PropietarioId") && row["PropietarioId"] != DBNull.Value)
                _propietarioIdSeleccionado = Convert.ToInt32(row["PropietarioId"]);

            if (row.Table.Columns.Contains("PropietarioNombre") && row["PropietarioNombre"] != DBNull.Value)
                txtNombrePropietario.Text = Convert.ToString(row["PropietarioNombre"]);

            if (row.Table.Columns.Contains("PropietarioDocumento") && row["PropietarioDocumento"] != DBNull.Value)
                txtPropietarioDocumento.Text = Convert.ToString(row["PropietarioDocumento"]);

            if (row.Table.Columns.Contains("Correo") && row["Correo"] != DBNull.Value)
                txtCorreo.Text = Convert.ToString(row["Correo"]);

            if (row.Table.Columns.Contains("Porcentaje") && row["Porcentaje"] != DBNull.Value)
                txtPorcentaje.Text = Convert.ToDecimal(row["Porcentaje"]).ToString(CultureInfo.CurrentCulture);

            if (row.Table.Columns.Contains("EsTitularPrincipal") && row["EsTitularPrincipal"] != DBNull.Value)
                chkTitular.Checked = Convert.ToBoolean(row["EsTitularPrincipal"]);

            if (row.Table.Columns.Contains("FechaInicio") && row["FechaInicio"] != DBNull.Value)
                dtpFechaInicio.Value = Convert.ToDateTime(row["FechaInicio"]);

            if (row.Table.Columns.Contains("FechaFin") && row["FechaFin"] != DBNull.Value)
                dtpFechaFin.Value = Convert.ToDateTime(row["FechaFin"]);
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            using (var dlg = new BuscarPropietario())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                _propietarioIdSeleccionado = dlg.SelectedId;

                if (txtNombrePropietario != null)
                    txtNombrePropietario.Text = dlg.SelectedUsuario;

                if (txtPropietarioDocumento != null)
                    txtPropietarioDocumento.Text = dlg.SelectedDocumento;

                if (txtCorreo != null)
                    txtCorreo.Text = dlg.SelectedCorreo;
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombrePropiedad?.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("Ingrese el nombre de la propiedad.");

                if (nombre.Length > 50)
                    throw new InvalidOperationException("El nombre no puede exceder 50 caracteres.");

                if (!int.TryParse(txtIdUnidad?.Text?.Trim(), out int unidadId) || unidadId <= 0)
                    throw new InvalidOperationException("Ingrese el Id de la unidad válido.");

                if (_propietarioIdSeleccionado is null || _propietarioIdSeleccionado <= 0)
                    throw new InvalidOperationException("Seleccione un propietario con el botón 'Buscar Propietario'.");

                string sPct = txtPorcentaje?.Text?.Trim() ?? string.Empty;
                if (!decimal.TryParse(sPct, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal porcentaje) ||
                    porcentaje <= 0 || porcentaje > 100)
                {
                    throw new InvalidOperationException("Ingrese un porcentaje válido (0 < porcentaje ≤ 100).");
                }

                bool esTitular = chkTitular?.Checked ?? false;

                DateTime? fIni = dtpFechaInicio?.Value.Date;
                DateTime? fFin = dtpFechaFin?.Value.Date;

                if (fIni.HasValue && fFin.HasValue && fFin.Value <= fIni.Value)
                    throw new InvalidOperationException("La fecha de terminación debe ser mayor o igual que la fecha de inicio.");

                if (_rowVersion == null || _rowVersion.Length == 0)
                    throw new InvalidOperationException("No se pudo recuperar la versión de la fila (RowVersion).");

                string editor =
                    UserContext.Usuario ??
                    ConfigurationManager.AppSettings["DefaultEjecutor"] ??
                    "rtscon@local";

                _neg.Actualizar(
                    _id,
                    nombre,
                    _propietarioIdSeleccionado.Value,
                    unidadId,
                    fIni,
                    fFin,
                    porcentaje,
                    esTitular,
                    _rowVersion,
                    editor
                );

                KryptonMessageBox.Show(
                    this,
                    "Propiedad actualizada correctamente.",
                    "Actualizar Propiedad",
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
                    "Actualizar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }
    }
}