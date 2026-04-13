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
        private bool _eventosInicializados;

        public int BloqueIdSeleccionado { get; private set; }
        public string BloqueIdentificadorSeleccionado { get; private set; } = string.Empty;

        public BuscarBloque()
        {
            InitializeComponent();

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            dgvBloques.AutoGenerateColumns = true;

            Load -= BuscarBloque_Load;
            Load += BuscarBloque_Load;
        }

        private void BuscarBloque_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            chkSoloActivos.Checked = true;
            CargarBloques();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            btnConfirmar.Click -= btnConfirmar_Click;
            btnConfirmar.Click += btnConfirmar_Click;

            btnCancelar.Click -= btnCancelar_Click;
            btnCancelar.Click += btnCancelar_Click;

            btnLimpiarFiltros.Click -= btnLimpiarFiltros_Click;
            btnLimpiarFiltros.Click += btnLimpiarFiltros_Click;

            txtBuscar.KeyDown -= txtBuscar_KeyDown;
            txtBuscar.KeyDown += txtBuscar_KeyDown;

            txtBuscar.TextChanged -= txtBuscar_TextChanged;
            txtBuscar.TextChanged += txtBuscar_TextChanged;

            chkSoloActivos.CheckedChanged -= chkSoloActivos_CheckedChanged;
            chkSoloActivos.CheckedChanged += chkSoloActivos_CheckedChanged;

            dgvBloques.CellDoubleClick -= dgvBloques_CellDoubleClick;
            dgvBloques.CellDoubleClick += dgvBloques_CellDoubleClick;

            dgvBloques.MultiSelect = false;
            dgvBloques.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBloques.AllowUserToAddRows = false;
            dgvBloques.ReadOnly = true;
            dgvBloques.RowHeadersVisible = false;

            _eventosInicializados = true;
        }

        private void CargarBloques()
        {
            string texto = txtBuscar.Text.Trim();
            bool soloActivos = chkSoloActivos.Checked;

            DataTable dt = _nBloque.Buscar(null, texto, soloActivos, 50);
            dgvBloques.DataSource = dt;

            AjustarGrid();

            bool hay = dt != null && dt.Rows.Count > 0;
            lblSinResultados.Visible = !hay;
            btnConfirmar.Enabled = hay;
        }

        private void AjustarGrid()
        {
            if (dgvBloques.DataSource == null)
                return;

            if (dgvBloques.Columns.Contains("Id"))
                dgvBloques.Columns["Id"].Visible = false;

            if (dgvBloques.Columns.Contains("CondominioId"))
                dgvBloques.Columns["CondominioId"].Visible = false;

            if (dgvBloques.Columns.Contains("RowVersion"))
                dgvBloques.Columns["RowVersion"].Visible = false;

            if (dgvBloques.Columns.Contains("CreatedBy"))
                dgvBloques.Columns["CreatedBy"].Visible = false;

            if (dgvBloques.Columns.Contains("UpdatedBy"))
                dgvBloques.Columns["UpdatedBy"].Visible = false;

            if (dgvBloques.Columns.Contains("UpdatedAt"))
                dgvBloques.Columns["UpdatedAt"].Visible = false;

            if (dgvBloques.Columns.Contains("Identificador"))
                dgvBloques.Columns["Identificador"].HeaderText = "Bloque";

            if (dgvBloques.Columns.Contains("NumeroPisos"))
                dgvBloques.Columns["NumeroPisos"].HeaderText = "Pisos";

            if (dgvBloques.Columns.Contains("UnidadesPorPiso"))
                dgvBloques.Columns["UnidadesPorPiso"].HeaderText = "Unidades por Piso";

            if (dgvBloques.Columns.Contains("IsActive"))
                dgvBloques.Columns["IsActive"].HeaderText = "Activo";

            dgvBloques.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBloques.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvBloques.AllowUserToResizeRows = false;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            SeleccionarActual();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            chkSoloActivos.Checked = true;
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

        private void SeleccionarActual()
        {
            if (dgvBloques.CurrentRow == null)
                return;

            var rowView = dgvBloques.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
                return;

            BloqueIdSeleccionado = Convert.ToInt32(rowView["Id"]);
            BloqueIdentificadorSeleccionado = Convert.ToString(rowView["Identificador"]) ?? string.Empty;

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