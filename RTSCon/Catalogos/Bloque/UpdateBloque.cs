using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class UpdateBloque : Form
    {
        private readonly int _id;
        private int _condominioId;
        private byte[] _rowVersion;

        private readonly NBloque _nBloque;
        private readonly NCondominio _nCondominio;

        public UpdateBloque(int id)
        {
            InitializeComponent();

            _id = id;

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            var dCondo = new DCondominio(Conexion.CadenaConexion);
            _nCondominio = new NCondominio(dCondo);
        }

        private void UpdateBloque_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            DataRow row = _nBloque.PorId(_id);
            if (row == null)
            {
                MessageBox.Show("No se encontró el bloque.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            _condominioId = (int)row["CondominioId"];
            _rowVersion = (byte[])row["RowVersion"];

            lblId.Text = _id.ToString();
            txtIdentificador.Text = row["Identificador"].ToString();

            // Asegúrate de que Maximum de los NumericUpDown sea >= a estos valores
            nudNumPisos.Value = Math.Min(nudNumPisos.Maximum, Math.Max(nudNumPisos.Minimum,
                                    Convert.ToInt32(row["NumeroPisos"])));

            nudUnidadesPiso.Value = Math.Min(nudUnidadesPiso.Maximum, Math.Max(nudUnidadesPiso.Minimum,
                                           Convert.ToInt32(row["UnidadesPorPiso"])));

            CargarCondominio();
        }

        private void CargarCondominio()
        {
            var rowCondo = _nCondominio.PorId(_condominioId);
            if (rowCondo != null)
                lblCondominio.Text = rowCondo["Nombre"].ToString();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string identificador = txtIdentificador.Text.Trim();
                int numeroPisos = (int)nudNumPisos.Value;
                int unidadesPorPiso = (int)nudUnidadesPiso.Value;

                if (numeroPisos <= 0)
                    throw new ArgumentException("El número de pisos debe ser mayor que 0.");
                if (unidadesPorPiso <= 0)
                    throw new ArgumentException("Las unidades por piso deben ser mayores que 0.");

                string usuario = UserContext.Usuario;

                _nBloque.Actualizar(_id, identificador, numeroPisos, unidadesPorPiso, _rowVersion, usuario);

                MessageBox.Show("Bloque actualizado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo actualizar el bloque: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnBuscarCondominio_Click(object sender, EventArgs e)
        {
            using (var frm = new BuscarCondominio())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _condominioId = frm.CondominioIdSeleccionado;
                    CargarCondominio();
                }
            }
        }

        private void btnBuscarCondominio_Click_1(object sender, EventArgs e)
        {
            using (var frm = new BuscarCondominio())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _condominioId = frm.CondominioIdSeleccionado;
                    lblCondominio.Text = frm.CondominioNombreSeleccionado;
                }
            }
        }
    }
}
