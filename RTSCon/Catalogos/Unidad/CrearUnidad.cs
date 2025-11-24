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

        public CrearUnidad(int bloqueId)
        {
            InitializeComponent();

            _bloqueId = bloqueId;

            var dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);
        }

        private void CrearUnidad_Load(object sender, EventArgs e)
        {
            CargarBloque();
        }

        private void CargarBloque()
        {
            if (_bloqueId <= 0) return;

            var row = _nBloque.PorId(_bloqueId);
            if (row != null)
            {
                // muestra el identificador del bloque en un label
                lblBloque.Text = row["Identificador"].ToString();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string numero = txtNumero.Text.Trim();
                string tipologia = txtTipologia.Text.Trim();
                string estacionamiento = txtEstacionamiento.Text.Trim();
                string observaciones = txtObservaciones.Text.Trim();

                // Piso (int)
                if (!int.TryParse(txtPiso.Text.Trim(), out int piso) || piso < 0)
                {
                    MessageBox.Show("El piso debe ser un número entero (>= 0).", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPiso.Focus();
                    return;
                }

                // Metros2 (decimal?)
                decimal? metros2 = null;
                if (!string.IsNullOrWhiteSpace(txtMetros2.Text))
                {
                    if (!decimal.TryParse(txtMetros2.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var m2))
                    {
                        MessageBox.Show("Metros2 debe ser un número decimal válido.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMetros2.Focus();
                        return;
                    }
                    metros2 = m2;
                }

                // Amueblada / CantidadMuebles
                bool? amueblada = chkAmueblada.Checked;
                int? cantidadMuebles = null;
                if (!string.IsNullOrWhiteSpace(txtCantidadMuebles.Text))
                {
                    if (!int.TryParse(txtCantidadMuebles.Text.Trim(), out var cm) || cm < 0)
                    {
                        MessageBox.Show("Cantidad de muebles debe ser un número entero (>= 0).", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCantidadMuebles.Focus();
                        return;
                    }
                    cantidadMuebles = cm;
                }

                // CuotaMantenimientoEspecifica (decimal?)
                decimal? cuotaEspecifica = null;
                if (!string.IsNullOrWhiteSpace(txtCuotaMantenimientoEspecifica.Text))
                {
                    if (!decimal.TryParse(txtCuotaMantenimientoEspecifica.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var cu))
                    {
                        MessageBox.Show("La cuota específica debe ser un número decimal válido.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCuotaMantenimientoEspecifica.Focus();
                        return;
                    }
                    cuotaEspecifica = cu;
                }

                string usuario = UserContext.Usuario;

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

                MessageBox.Show("Unidad creada correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo crear la unidad: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CrearUnidad_Load_1(object sender, EventArgs e)
        {

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

        }
    }
}
