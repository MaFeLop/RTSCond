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
        private bool _eventosInicializados;

        public int CondominioIdSeleccionado { get; private set; }
        public string CondominioNombreSeleccionado { get; private set; } = string.Empty;

        public BuscarCondominio()
        {
            InitializeComponent();

            var dCondo = new DCondominio(Conexion.CadenaConexion);
            _nCondominio = new NCondominio(dCondo);

            dgvCondominios.AutoGenerateColumns = true;

            Load -= BuscarCondominio_Load;
            Load += BuscarCondominio_Load;
        }

        private void BuscarCondominio_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            chkSoloActivos.Checked = true;
            CargarCondominios();
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

            dgvCondominios.CellDoubleClick -= dgvCondominios_CellDoubleClick;
            dgvCondominios.CellDoubleClick += dgvCondominios_CellDoubleClick;

            dgvCondominios.MultiSelect = false;
            dgvCondominios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCondominios.AllowUserToAddRows = false;
            dgvCondominios.ReadOnly = true;
            dgvCondominios.RowHeadersVisible = false;

            _eventosInicializados = true;
        }

        private void CargarCondominios()
        {
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                DataTable dt = _nCondominio.Buscar(texto, soloActivos, 50);
                dgvCondominios.DataSource = dt;
                AjustarGrid();

                btnConfirmar.Enabled = dt != null && dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar condominios: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AjustarGrid()
        {
            if (dgvCondominios.DataSource == null)
                return;

            if (dgvCondominios.Columns.Contains("Id"))
                dgvCondominios.Columns["Id"].Visible = false;

            if (dgvCondominios.Columns.Contains("RowVersion"))
                dgvCondominios.Columns["RowVersion"].Visible = false;

            if (dgvCondominios.Columns.Contains("CreatedBy"))
                dgvCondominios.Columns["CreatedBy"].Visible = false;

            if (dgvCondominios.Columns.Contains("UpdatedBy"))
                dgvCondominios.Columns["UpdatedBy"].Visible = false;

            if (dgvCondominios.Columns.Contains("UpdatedAt"))
                dgvCondominios.Columns["UpdatedAt"].Visible = false;

            if (dgvCondominios.Columns.Contains("Nombre"))
                dgvCondominios.Columns["Nombre"].HeaderText = "Condominio";

            if (dgvCondominios.Columns.Contains("Direccion"))
                dgvCondominios.Columns["Direccion"].HeaderText = "Dirección";

            if (dgvCondominios.Columns.Contains("Tipo"))
                dgvCondominios.Columns["Tipo"].HeaderText = "Tipo";

            if (dgvCondominios.Columns.Contains("AdministradorResponsable"))
                dgvCondominios.Columns["AdministradorResponsable"].HeaderText = "Propietario Responsable";

            if (dgvCondominios.Columns.Contains("IsActive"))
                dgvCondominios.Columns["IsActive"].HeaderText = "Activo";

            dgvCondominios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCondominios.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvCondominios.AllowUserToResizeRows = false;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarCondominios();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (dgvCondominios.CurrentRow == null)
            {
                MessageBox.Show(
                    "Selecciona un condominio.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var view = dgvCondominios.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
                return;

            CondominioIdSeleccionado = Convert.ToInt32(view["Id"]);
            CondominioNombreSeleccionado = Convert.ToString(view["Nombre"]) ?? string.Empty;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            chkSoloActivos.Checked = true;
            CargarCondominios();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CargarCondominios();
            }
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            CargarCondominios();
        }

        private void dgvCondominios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnConfirmar_Click(sender, EventArgs.Empty);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}