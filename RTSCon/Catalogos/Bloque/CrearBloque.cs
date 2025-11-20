using System;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class CrearBloque : Form
    {
        private int _condominioId;
        private readonly NBloque _nBloque;
        private readonly NCondominio _nCondominio;

        public CrearBloque(int condominioId)
        {
            InitializeComponent();

            _condominioId = condominioId;

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            var dCondo = new DCondominio(Conexion.CadenaConexion);
            _nCondominio = new NCondominio(dCondo);
        }

        private void CrearBloque_Load(object sender, EventArgs e)
        {
            CargarCondominio();
        }

        private void CargarCondominio()
        {
            if (_condominioId <= 0) return;

            var row = _nCondominio.PorId(_condominioId);
            if (row != null)
                lblCondominio.Text = row["Nombre"].ToString();
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

                string usuario = UserContext.Usuario;  // o lo que uses

                _nBloque.Insertar(_condominioId, identificador, numeroPisos, unidadesPorPiso, usuario);

                MessageBox.Show("Bloque creado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo crear el bloque: " + ex.Message,
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
                    _condominioId = frm.CondominioIdSeleccionado; // lo haces luego
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
