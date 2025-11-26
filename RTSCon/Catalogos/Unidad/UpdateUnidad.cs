using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class UpdateUnidad : Form
    {
        private readonly int _id;
        private int _bloqueId;
        private byte[] _rowVersion;

        private readonly NUnidad _nUnidad;
        private readonly NBloque _nBloque;

        public UpdateUnidad(int id)
        {
            InitializeComponent();

            _id = id;

            var dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);
        }

        private void UpdateUnidad_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            DataRow row = _nUnidad.PorId(_id);
            if (row == null)
            {
                MessageBox.Show("No se encontró la unidad.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            _bloqueId = (int)row["BloqueId"];
            _rowVersion = (byte[])row["RowVersion"];

            CargarBloque();   // aquí se pone correctamente el Identificador
        }

        private void CargarBloque()
        {
            var rowBloque = _nBloque.PorId(_bloqueId);
            if (rowBloque != null)
                lblBloque.Text = rowBloque["Identificador"].ToString();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string numero = txtNumero.Text.Trim();
                string tipologia = txtTipologia.Text.Trim();
                string estacionamiento = txtEstacionamiento.Text.Trim();
                string observaciones = txtObservaciones.Text.Trim();

                if (!int.TryParse(txtPiso.Text.Trim(), out int piso) || piso < 0)
                {
                    MessageBox.Show("El piso debe ser un número entero (>= 0).", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPiso.Focus();
                    return;
                }

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

                _nUnidad.Actualizar(
                    _id,
                    numero,
                    piso,
                    tipologia,
                    metros2,
                    estacionamiento,
                    amueblada,
                    cantidadMuebles,
                    cuotaEspecifica,
                    observaciones,
                    _rowVersion,
                    usuario);

                MessageBox.Show("Unidad actualizada correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo actualizar la unidad: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lblBloque_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscarBloque_Click(object sender, EventArgs e)
        {
            using (var frm = new BuscarBloque())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _bloqueId = frm.BloqueIdSeleccionado;
                    lblBloque.Text = frm.BloqueIdentificadorSeleccionado;
                }
            }
        }
    }
}
