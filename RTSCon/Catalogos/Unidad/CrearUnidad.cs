using System;
using System.Globalization;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class CrearUnidad : Form
    {
        private int _bloqueId;
        private readonly NUnidad _nUnidad;
        private readonly NBloque _nBloque;
        private bool _eventosInicializados;

        public CrearUnidad(int bloqueId)
        {
            InitializeComponent();

            _bloqueId = bloqueId;

            var dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            InicializarEventosUnaSolaVez();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            this.Load -= CrearUnidad_Load_1;
            this.Load += CrearUnidad_Load_1;

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

            if (btnBuscarBloque != null)
            {
                btnBuscarBloque.Click -= btnBuscarBloque_Click;
                btnBuscarBloque.Click += btnBuscarBloque_Click;
            }

            if (chkAmueblada != null)
            {
                chkAmueblada.CheckedChanged -= chkTitular_CheckedChanged;
                chkAmueblada.CheckedChanged += chkTitular_CheckedChanged;
            }

            _eventosInicializados = true;
        }

        private void CrearUnidad_Load(object sender, EventArgs e)
        {
            InicializarFormulario();
        }

        private void CrearUnidad_Load_1(object sender, EventArgs e)
        {
            InicializarFormulario();
        }

        private void InicializarFormulario()
        {
            CargarBloque();
            ActualizarEstadoMuebles();

            if (txtUnidadEnlazada != null)
                txtUnidadEnlazada.ReadOnly = true;

            if (txtMetros2 != null)
                txtMetros2.ReadOnly = false;

            if (txtEstacionamiento != null)
                txtEstacionamiento.ReadOnly = false;
        }

        private void CargarBloque()
        {
            if (_bloqueId <= 0)
            {
                if (txtUnidadEnlazada != null)
                    txtUnidadEnlazada.Text = string.Empty;

                return;
            }

            try
            {
                var row = _nBloque.PorId(_bloqueId);
                if (row != null)
                {
                    string identificador = Convert.ToString(row["Identificador"]) ?? string.Empty;

                    if (txtUnidadEnlazada != null)
                        txtUnidadEnlazada.Text = identificador;
                }
                else
                {
                    if (txtUnidadEnlazada != null)
                        txtUnidadEnlazada.Text = string.Empty;
                }
            }
            catch
            {
                if (txtUnidadEnlazada != null)
                    txtUnidadEnlazada.Text = string.Empty;
            }
        }

        private void ActualizarEstadoMuebles()
        {
            bool visible = chkAmueblada != null && chkAmueblada.Checked;

            if (lblMuebles != null)
                lblMuebles.Visible = visible;

            if (txtCantidadMuebles != null)
            {
                txtCantidadMuebles.Visible = visible;

                if (!visible)
                    txtCantidadMuebles.Text = string.Empty;
            }
        }

        private bool TryParseDecimalFlexible(string texto, out decimal valor)
        {
            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
                return true;

            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                return true;

            return false;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bloqueId <= 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar un bloque.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string numero = txtNumero.Text.Trim();
                string tipologia = txtTipologia.Text.Trim();
                string estacionamiento = txtEstacionamiento.Text.Trim();
                string observaciones = txtObservaciones.Text.Trim();

                if (string.IsNullOrWhiteSpace(numero))
                {
                    MessageBox.Show(
                        "Debe ingresar el número de la unidad.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtNumero.Focus();
                    return;
                }

                int piso;
                if (!int.TryParse(txtPiso.Text.Trim(), out piso) || piso < 0)
                {
                    MessageBox.Show(
                        "El piso debe ser un número entero (>= 0).",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtPiso.Focus();
                    return;
                }

                decimal? metros2 = null;
                if (!string.IsNullOrWhiteSpace(txtMetros2.Text))
                {
                    decimal m2;
                    if (!TryParseDecimalFlexible(txtMetros2.Text.Trim(), out m2))
                    {
                        MessageBox.Show(
                            "Metros cuadrados debe ser un número decimal válido.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtMetros2.Focus();
                        return;
                    }

                    metros2 = m2;
                }

                bool? amueblada = chkAmueblada.Checked;
                int? cantidadMuebles = null;

                if (chkAmueblada.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txtCantidadMuebles.Text))
                    {
                        MessageBox.Show(
                            "Debe indicar la cantidad de muebles.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtCantidadMuebles.Focus();
                        return;
                    }

                    int cm;
                    if (!int.TryParse(txtCantidadMuebles.Text.Trim(), out cm) || cm < 0)
                    {
                        MessageBox.Show(
                            "Cantidad de muebles debe ser un número entero (>= 0).",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtCantidadMuebles.Focus();
                        return;
                    }

                    cantidadMuebles = cm;
                }

                decimal? cuotaEspecifica = null;
                if (!string.IsNullOrWhiteSpace(txtCuotaMantenimientoEspecifica.Text))
                {
                    decimal cu;
                    if (!TryParseDecimalFlexible(txtCuotaMantenimientoEspecifica.Text.Trim(), out cu))
                    {
                        MessageBox.Show(
                            "La cuota de mantenimiento debe ser un número decimal válido.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtCuotaMantenimientoEspecifica.Focus();
                        return;
                    }

                    cuotaEspecifica = cu;
                }

                string usuario = UserContext.Usuario;
                if (string.IsNullOrWhiteSpace(usuario))
                    usuario = "rtscon@local";

                _nUnidad.Insertar(
                    _bloqueId,
                    numero,
                    piso,
                    tipologia,
                    metros2,
                    estacionamiento,
                    amueblada,
                    cantidadMuebles,
                    cuotaEspecifica,
                    observaciones,
                    usuario);

                MessageBox.Show(
                    "Unidad creada correctamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo crear la unidad: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void txtIdPropietario_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtObservaciones_TextChanged(object sender, EventArgs e)
        {
        }

        private void chkTitular_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoMuebles();
        }

        private void btnBuscarBloque_Click(object sender, EventArgs e)
        {
            using (var frm = new BuscarBloque())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _bloqueId = frm.BloqueIdSeleccionado;
                    CargarBloque();
                }
            }
        }
    }
}