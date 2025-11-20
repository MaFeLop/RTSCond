using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;
using Krypton.Toolkit;

namespace RTSCon.Catalogos
{
    public partial class BuscarCondominio : KryptonForm
    {
        private readonly NCondominio _nCondominio;

        public int CondominioIdSeleccionado { get; private set; }
        public string CondominioNombreSeleccionado { get; private set; }

        public BuscarCondominio()
        {
            InitializeComponent();

            var dCondo = new DCondominio(Conexion.CadenaConexion);
            _nCondominio = new NCondominio(dCondo);

            dgvCondominios.AutoGenerateColumns = false;
        }

        private void BuscarCondominio_Load(object sender, EventArgs e)
        {
            chkSoloActivos.Checked = true;
            CargarCondominios();
        }

        private void CargarCondominios()
        {
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                // top 50, mismo patrón que NCondominio.Buscar
                DataTable dt = _nCondominio.Buscar(texto, soloActivos, 50);
                dgvCondominios.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar condominios: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => CargarCondominios();

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CargarCondominios();
            }
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e) => CargarCondominios();

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (dgvCondominios.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un condominio.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var view = dgvCondominios.CurrentRow.DataBoundItem as DataRowView;
            if (view == null) return;

            CondominioIdSeleccionado = (int)view["Id"];
            CondominioNombreSeleccionado = view["Nombre"].ToString();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void dgvCondominios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnAceptar_Click(sender, EventArgs.Empty);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
