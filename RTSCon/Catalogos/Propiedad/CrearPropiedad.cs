using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class CrearPropiedad : Form
    {
        private readonly NPropiedad _neg;
        private int? _propietarioIdSeleccionado;
        private int? _unidadIdSeleccionada;
        private bool _eventosInicializados;

        public CrearPropiedad()
        {
            InitializeComponent();

            string cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            InicializarEventosUnaSolaVez();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            Shown -= CrearPropiedad_Shown;
            Shown += CrearPropiedad_Shown;

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

            _eventosInicializados = true;
        }

        private void CrearPropiedad_Shown(object sender, EventArgs e)
        {
            InitUi();
        }

        private void InitUi()
        {
            if (txtNombrePropiedad != null)
                txtNombrePropiedad.ReadOnly = false;

            if (txtNombrePropietario != null)
                txtNombrePropietario.ReadOnly = true;

            if (txtPropietarioDocumento != null)
                txtPropietarioDocumento.ReadOnly = true;

            if (txtIdUnidad != null)
                txtIdUnidad.ReadOnly = true;

            if (txtCorreoNotificacion != null)
                txtCorreoNotificacion.ReadOnly = true;

            DateTime hoy = DateTime.Today;

            if (dtpFechaInicio != null)
                dtpFechaInicio.Value = hoy;

            if (dtpFechaFin != null)
                dtpFechaFin.Value = hoy.AddDays(1);

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

            dtpFechaFin.MinDate = minimoFin;

            if (dtpFechaFin.Value.Date < minimoFin)
                dtpFechaFin.Value = minimoFin;
        }

        private bool ValidarFechas(out DateTime fechaInicio, out DateTime fechaFin, out string mensaje)
        {
            fechaInicio = DateTime.MinValue;
            fechaFin = DateTime.MinValue;
            mensaje = string.Empty;

            if (dtpFechaInicio == null || dtpFechaFin == null)
            {
                mensaje = "No se pudieron leer las fechas del formulario.";
                return false;
            }

            fechaInicio = dtpFechaInicio.Value.Date;
            fechaFin = dtpFechaFin.Value.Date;

            if (fechaFin <= fechaInicio)
            {
                mensaje = "La fecha de terminación debe ser posterior a la fecha de inicio; no puede vencer el mismo día.";
                return false;
            }

            return true;
        }

        private bool TryParseDecimalFlexible(string texto, out decimal valor)
        {
            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
                return true;

            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                return true;

            return false;
        }

        private void dtpFechaInicio_ValueChanged(object sender, EventArgs e)
        {
            AplicarReglaFechas();
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

                if (txtCorreoNotificacion != null)
                    txtCorreoNotificacion.Text = dlg.SelectedCorreo;
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
            try
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
                DateTime fechaFin;
                string mensajeFechas;

                if (!ValidarFechas(out fechaInicio, out fechaFin, out mensajeFechas))
                    throw new InvalidOperationException(mensajeFechas);

                string creador =
                    UserContext.Usuario ??
                    ConfigurationManager.AppSettings["DefaultEjecutor"] ??
                    "rtscon@local";

                int nuevoId = _neg.Insertar(
                    nombre,
                    _propietarioIdSeleccionado.Value,
                    _unidadIdSeleccionada.Value,
                    fechaInicio,
                    fechaFin,
                    porcentaje,
                    esTitular,
                    creador
                );

                KryptonMessageBox.Show(
                    this,
                    "Propiedad registrada (Id " + nuevoId + ").",
                    "Crear Propiedad",
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
                    "Crear Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }
    }
}