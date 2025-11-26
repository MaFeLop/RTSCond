using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;
using Krypton.Toolkit;

namespace RTSCon.Catalogos
{
    public partial class BuscarBloque : KryptonForm
    {
        private readonly NBloque _nBloque;

        public int BloqueIdSeleccionado { get; private set; }
        public string BloqueIdentificadorSeleccionado { get; private set; }

        public BuscarBloque()
        {
            InitializeComponent();

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            // Si diseñaste las columnas a mano en el diseñador:
            dgvBloques.AutoGenerateColumns = false;
        }

        private void BuscarBloque_Load(object sender, EventArgs e)
        {
            chkSoloActivos.Checked = true;   // si tienes este checkbox
            CargarBloques();
        }

        private void CargarBloques()
        {
            string texto = txtBuscar.Text.Trim();
            bool soloActivos = chkSoloActivos.Checked;   // si no lo tienes, pon soloActivos=false

            // Si no filtras por condominio aquí, pasa null
            DataTable dt = _nBloque.Buscar(null, texto, soloActivos, 50);
            dgvBloques.DataSource = dt;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarBloques();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CargarBloques();
            }
        }

        private void dgvBloques_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                SeleccionarActual();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            SeleccionarActual();
        }

        private void SeleccionarActual()
        {
            if (dgvBloques.CurrentRow == null)
                return;

            var rowView = dgvBloques.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
                return;

            BloqueIdSeleccionado = (int)rowView["Id"];
            BloqueIdentificadorSeleccionado = rowView["Identificador"].ToString();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
