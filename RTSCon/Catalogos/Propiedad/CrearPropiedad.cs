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

        public CrearPropiedad()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            this.Shown += (_, __) => InitUi();

            if (btnConfirmar != null)
                btnConfirmar.Click += btnConfirmar_Click;

            if (btnVolver != null)
                btnVolver.Click += (_, __) => Close();

            if (btnBuscarPropietario != null)
                btnBuscarPropietario.Click += btnBuscarPropietario_Click;

            SetKeyPressDecimal(txtPorcentaje);

            if (txtIdPropiedad != null)
            {
                txtIdPropiedad.KeyDown += (s, e) =>
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

        private void InitUi()
        {
            if (dtpFechaInicio != null)
                dtpFechaInicio.Value = DateTime.Today;

            if (dtpFechaFin != null)
                dtpFechaFin.Value = DateTime.Today;

            if (txtNombrePropiedad != null)
                txtNombrePropiedad.ReadOnly = false;

            if (txtNombrePropietario != null)
                txtNombrePropietario.ReadOnly = true;

            if (txtIdPropietario != null)
                txtIdPropietario.ReadOnly = true;
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            using (var dlg = new RTSCon.Catalogos.BuscarPropietario())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                _propietarioIdSeleccionado = dlg.SelectedId;
                txtNombrePropietario.Text = dlg.SelectedUsuario;
                txtIdPropietario.Text = dlg.SelectedDocumento;
                txtCorreo.Text = dlg.SelectedCorreo;
            }
        }

        private void btnBuscarPropietario_Click_1(object sender, EventArgs e)
        {
            btnBuscarPropietario_Click(sender, e);
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

                if (!int.TryParse(txtIdPropiedad?.Text?.Trim(), out int unidadId) || unidadId <= 0)
                    throw new InvalidOperationException("Ingrese el Id de la unidad (vivienda) válido.");

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

                if (fIni.HasValue && fFin.HasValue && fIni.Value > fFin.Value)
                    throw new InvalidOperationException("La fecha de inicio no puede ser mayor que la fecha de terminación.");

                string creador = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                int nuevoId = _neg.Insertar(
                    nombre,
                    _propietarioIdSeleccionado.Value,
                    unidadId,
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

                this.DialogResult = DialogResult.OK;
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