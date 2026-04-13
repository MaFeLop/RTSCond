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

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            Shown -= CrearPropiedad_Shown;
            Shown += CrearPropiedad_Shown;
        }

        private void CrearPropiedad_Shown(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            InitUi();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            btnConfirmar.Click -= btnConfirmar_Click;
            btnConfirmar.Click += btnConfirmar_Click;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            btnBuscarPropietario.Click -= btnBuscarPropietario_Click;
            btnBuscarPropietario.Click += btnBuscarPropietario_Click;

            btnBuscarUnidad.Click -= btnBuscarUnidad_Click;
            btnBuscarUnidad.Click += btnBuscarUnidad_Click;

            SetKeyPressDecimal(txtPorcentaje);

            if (txtIdUnidad != null)
            {
                txtIdUnidad.KeyDown -= txtIdUnidad_KeyDown;
                txtIdUnidad.KeyDown += txtIdUnidad_KeyDown;
            }

            if (txtPorcentaje != null)
            {
                txtPorcentaje.KeyDown -= txtPorcentaje_KeyDown;
                txtPorcentaje.KeyDown += txtPorcentaje_KeyDown;
            }

            _eventosInicializados = true;
        }

        private void SetKeyPressDecimal(KryptonTextBox tb)
        {
            if (tb == null)
                return;

            tb.KeyPress -= Decimal_KeyPress;
            tb.KeyPress += Decimal_KeyPress;
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
                btnConfirmar.PerformClick();
            }
        }

        private void txtPorcentaje_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnConfirmar.PerformClick();
            }
        }

        private void InitUi()
        {
            dtpFechaInicio.Value = DateTime.Today;
            dtpFechaFin.Value = DateTime.Today;

            txtNombrePropiedad.ReadOnly = false;
            txtNombrePropietario.ReadOnly = true;
            txtPropietarioDocumento.ReadOnly = true;
            txtIdUnidad.ReadOnly = true;
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            using (var dlg = new BuscarPropietario())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                _propietarioIdSeleccionado = dlg.SelectedId;

                txtNombrePropietario.Text = dlg.SelectedUsuario;
                txtPropietarioDocumento.Text = dlg.SelectedDocumento;
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
                string nombre = txtNombrePropiedad?.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("Ingrese el nombre de la propiedad.");

                if (nombre.Length > 50)
                    throw new InvalidOperationException("El nombre no puede exceder 50 caracteres.");

                if (_unidadIdSeleccionada is null || _unidadIdSeleccionada <= 0)
                    throw new InvalidOperationException("Seleccione una unidad con el botón 'Buscar Unidad'.");

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

                if (fIni.HasValue && fFin.HasValue && fFin.Value < fIni.Value)
                    throw new InvalidOperationException("La fecha de terminación no puede ser menor que la fecha de inicio.");

                string creador = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                int nuevoId = _neg.Insertar(
                    nombre,
                    _propietarioIdSeleccionado.Value,
                    _unidadIdSeleccionada.Value,
                    fIni,
                    fFin,
                    porcentaje,
                    esTitular,
                    creador
                );

                KryptonMessageBox.Show(
                    this,
                    $"Propiedad registrada (Id {nuevoId}).",
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