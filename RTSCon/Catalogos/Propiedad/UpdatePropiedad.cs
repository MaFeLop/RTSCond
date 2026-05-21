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
        private int? _unidadIdSeleccionada;

        private bool _eventosInicializados;
        private bool _guardando;

        public UpdatePropiedad()
        {
            InitializeComponent();

            string cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            InicializarEventosUnaSolaVez();
        }

        public UpdatePropiedad(int id)
            : this()
        {
            Tag = id;
        }

        #region Configuración inicial

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            Shown -= UpdatePropiedad_Shown;
            Shown += UpdatePropiedad_Shown;

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

            if (btnBuscarUnidad != null)
            {
                btnBuscarUnidad.Click -= btnBuscarUnidad_Click;
                btnBuscarUnidad.Click += btnBuscarUnidad_Click;
            }

            if (txtIdUnidad != null)
            {
                txtIdUnidad.KeyDown -= txtIdUnidad_KeyDown;
                txtIdUnidad.KeyDown += txtIdUnidad_KeyDown;
            }

            if (txtPorcentaje != null)
            {
                txtPorcentaje.KeyDown -= txtPorcentaje_KeyDown;
                txtPorcentaje.KeyDown += txtPorcentaje_KeyDown;

                txtPorcentaje.KeyPress -= Decimal_KeyPress;
                txtPorcentaje.KeyPress += Decimal_KeyPress;
            }

            if (dtpFechaInicio != null)
            {
                dtpFechaInicio.ValueChanged -= dtpFechaInicio_ValueChanged;
                dtpFechaInicio.ValueChanged += dtpFechaInicio_ValueChanged;
            }

            if (chkRentado != null)
            {
                chkRentado.CheckedChanged -= chkRentado_CheckedChanged;
                chkRentado.CheckedChanged += chkRentado_CheckedChanged;
            }

            _eventosInicializados = true;
        }

        private void UpdatePropiedad_Shown(object sender, EventArgs e)
        {
            InitUiAndLoad();
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
                if (Tag is int)
                    _id = (int)Tag;
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

            if (txtNombrePropiedad != null)
                txtNombrePropiedad.ReadOnly = false;

            if (txtNombrePropietario != null)
                txtNombrePropietario.ReadOnly = true;

            if (txtPropietarioDocumento != null)
                txtPropietarioDocumento.ReadOnly = true;

            if (txtIdUnidad != null)
                txtIdUnidad.ReadOnly = true;

            if (txtCorreo != null)
                txtCorreo.ReadOnly = true;

            if (dtpFechaInicio != null)
                dtpFechaInicio.Value = DateTime.Today;

            if (dtpFechaFin != null)
                dtpFechaFin.Value = DateTime.Today.AddDays(1);

            CargarDatos();
            ActualizarEstadoFechaFin();
        }

        #endregion

        #region Tipo de tenencia / fechas

        private bool EstaRentada()
        {
            return chkRentado != null && chkRentado.Checked;
        }

        private string ObtenerTipoTenencia()
        {
            return EstaRentada() ? "Rentada" : "Comprada";
        }

        private void ActualizarEstadoFechaFin()
        {
            bool rentada = EstaRentada();

            if (label6 != null)
                label6.Visible = rentada;

            if (dtpFechaFin != null)
                dtpFechaFin.Visible = rentada;

            if (rentada)
                AplicarReglaFechas();
        }

        private void AplicarReglaFechas()
        {
            if (dtpFechaInicio == null || dtpFechaFin == null)
                return;

            DateTime minimoFin = dtpFechaInicio.Value.Date.AddDays(1);

            if (minimoFin < dtpFechaFin.MinDate)
                minimoFin = dtpFechaFin.MinDate;

            if (minimoFin > dtpFechaFin.MaxDate)
                minimoFin = dtpFechaFin.MaxDate;

            if (dtpFechaFin.Value.Date < minimoFin)
                dtpFechaFin.Value = minimoFin;

            dtpFechaFin.MinDate = minimoFin;
        }

        private bool ValidarFechas(out DateTime fechaInicio, out DateTime? fechaFin, out string mensaje)
        {
            fechaInicio = DateTime.MinValue;
            fechaFin = null;
            mensaje = string.Empty;

            if (dtpFechaInicio == null)
            {
                mensaje = "No se pudo leer la fecha de inicio del formulario.";
                return false;
            }

            fechaInicio = dtpFechaInicio.Value.Date;

            if (!EstaRentada())
            {
                fechaFin = null;
                return true;
            }

            if (dtpFechaFin == null)
            {
                mensaje = "No se pudo leer la fecha de terminación del formulario.";
                return false;
            }

            fechaFin = dtpFechaFin.Value.Date;

            if (fechaFin.Value <= fechaInicio)
            {
                mensaje = "La fecha de terminación debe ser posterior a la fecha de inicio; no puede vencer el mismo día.";
                return false;
            }

            return true;
        }

        #endregion

        #region Carga de datos

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
            {
                _unidadIdSeleccionada = Convert.ToInt32(row["UnidadId"]);
                txtIdUnidad.Text = Convert.ToString(row["UnidadId"]);
            }

            if (row.Table.Columns.Contains("PropietarioId") && row["PropietarioId"] != DBNull.Value)
                _propietarioIdSeleccionado = Convert.ToInt32(row["PropietarioId"]);

            if (row.Table.Columns.Contains("PropietarioNombre") && row["PropietarioNombre"] != DBNull.Value)
                txtNombrePropietario.Text = Convert.ToString(row["PropietarioNombre"]);

            if (row.Table.Columns.Contains("PropietarioDocumento") && row["PropietarioDocumento"] != DBNull.Value)
                txtPropietarioDocumento.Text = Convert.ToString(row["PropietarioDocumento"]);

            if (row.Table.Columns.Contains("Correo") && row["Correo"] != DBNull.Value && txtCorreo != null)
                txtCorreo.Text = Convert.ToString(row["Correo"]);

            if (row.Table.Columns.Contains("Porcentaje") && row["Porcentaje"] != DBNull.Value)
                txtPorcentaje.Text = Convert.ToDecimal(row["Porcentaje"]).ToString(CultureInfo.CurrentCulture);

            if (row.Table.Columns.Contains("EsTitularPrincipal") && row["EsTitularPrincipal"] != DBNull.Value)
                chkTitular.Checked = Convert.ToBoolean(row["EsTitularPrincipal"]);

            DateTime fechaInicio = DateTime.Today;
            DateTime fechaFin = DateTime.Today.AddDays(1);

            if (row.Table.Columns.Contains("FechaInicio") && row["FechaInicio"] != DBNull.Value)
                fechaInicio = Convert.ToDateTime(row["FechaInicio"]).Date;

            if (row.Table.Columns.Contains("FechaFin") && row["FechaFin"] != DBNull.Value)
                fechaFin = Convert.ToDateTime(row["FechaFin"]).Date;
            else
                fechaFin = fechaInicio.AddDays(1);

            if (fechaFin <= fechaInicio)
                fechaFin = fechaInicio.AddDays(1);

            if (dtpFechaInicio != null)
                dtpFechaInicio.Value = fechaInicio;

            if (dtpFechaFin != null)
                dtpFechaFin.Value = fechaFin;

            string tipoTenencia = "Rentada";

            if (row.Table.Columns.Contains("TipoTenencia") && row["TipoTenencia"] != DBNull.Value)
                tipoTenencia = Convert.ToString(row["TipoTenencia"]);

            if (chkRentado != null)
                chkRentado.Checked = string.Equals(tipoTenencia, "Rentada", StringComparison.OrdinalIgnoreCase);

            ActualizarEstadoFechaFin();
        }

        #endregion

        #region Utilidades de validación

        private bool TryParseDecimalFlexible(string texto, out decimal valor)
        {
            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
                return true;

            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                return true;

            return false;
        }

        private void Decimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            KryptonTextBox tb = sender as KryptonTextBox;
            if (tb == null)
                return;

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
        }

        #endregion

        #region Eventos

        private void dtpFechaInicio_ValueChanged(object sender, EventArgs e)
        {
            if (EstaRentada())
                AplicarReglaFechas();
        }

        private void chkRentado_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoFechaFin();
        }

        private void txtIdUnidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnConfirmar?.PerformClick();
            }
        }

        private void txtPorcentaje_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnConfirmar?.PerformClick();
            }
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

        private void btnBuscarUnidad_Click(object sender, EventArgs e)
        {
            using (var dlg = new BuscarUnidad())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                _unidadIdSeleccionada = dlg.SelectedId;

                if (txtIdUnidad != null)
                    txtIdUnidad.Text = dlg.SelectedId.ToString();
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
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
                ActualizarPropiedad();
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
            finally
            {
                if (!IsDisposed && btnConfirmar != null)
                    btnConfirmar.Enabled = true;

                _guardando = false;
            }
        }

        #endregion

        #region Actualización

        private void ActualizarPropiedad()
        {
            string nombre = txtNombrePropiedad != null
                ? txtNombrePropiedad.Text.Trim()
                : string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
                throw new InvalidOperationException("Ingrese el nombre de la propiedad.");

            if (nombre.Length > 50)
                throw new InvalidOperationException("El nombre no puede exceder 50 caracteres.");

            if (_unidadIdSeleccionada == null || _unidadIdSeleccionada <= 0)
                throw new InvalidOperationException("Seleccione una unidad con el botón 'Buscar Unidad'.");

            if (_propietarioIdSeleccionado == null || _propietarioIdSeleccionado <= 0)
                throw new InvalidOperationException("Seleccione un propietario con el botón 'Buscar Propietario'.");

            string sPct = txtPorcentaje != null
                ? txtPorcentaje.Text.Trim()
                : string.Empty;

            decimal porcentaje;

            if (!TryParseDecimalFlexible(sPct, out porcentaje) || porcentaje <= 0 || porcentaje > 100)
                throw new InvalidOperationException("Ingrese un porcentaje válido (0 < porcentaje ≤ 100).");

            bool esTitular = chkTitular != null && chkTitular.Checked;

            DateTime fechaInicio;
            DateTime? fechaFin;
            string mensajeFechas;

            if (!ValidarFechas(out fechaInicio, out fechaFin, out mensajeFechas))
                throw new InvalidOperationException(mensajeFechas);

            if (_rowVersion == null || _rowVersion.Length == 0)
                throw new InvalidOperationException("No se pudo recuperar la versión de la fila (RowVersion).");

            string tipoTenencia = ObtenerTipoTenencia();

            string editor =
                UserContext.Usuario ??
                ConfigurationManager.AppSettings["DefaultEjecutor"] ??
                "rtscon@local";

            _neg.Actualizar(
                _id,
                nombre,
                _propietarioIdSeleccionado.Value,
                _unidadIdSeleccionada.Value,
                fechaInicio,
                fechaFin,
                porcentaje,
                esTitular,
                _rowVersion,
                tipoTenencia,
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

        #endregion
    }
}