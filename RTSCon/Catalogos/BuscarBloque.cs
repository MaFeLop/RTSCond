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
        private const string COL_SEL = "__sel";

        private readonly NBloque _nBloque;
        private DataTable _dt;
        private bool _eventosInicializados;

        public int BloqueIdSeleccionado { get; private set; }
        public string BloqueIdentificadorSeleccionado { get; private set; } = string.Empty;

        public BuscarBloque()
        {
            InitializeComponent();

            DBloque dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            Load -= BuscarBloque_Load;
            Load += BuscarBloque_Load;
        }

        private void BuscarBloque_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            chkSoloActivos.Checked = true;
            EnsureSelectionColumn();
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

            dgvBloques.CurrentCellDirtyStateChanged -= dgvBloques_CurrentCellDirtyStateChanged;
            dgvBloques.CurrentCellDirtyStateChanged += dgvBloques_CurrentCellDirtyStateChanged;

            dgvBloques.CellClick -= dgvBloques_CellClick;
            dgvBloques.CellClick += dgvBloques_CellClick;

            dgvBloques.CellValueChanged -= dgvBloques_CellValueChanged;
            dgvBloques.CellValueChanged += dgvBloques_CellValueChanged;

            dgvBloques.DataError -= dgvBloques_DataError;
            dgvBloques.DataError += dgvBloques_DataError;

            dgvBloques.MultiSelect = false;
            dgvBloques.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBloques.AllowUserToAddRows = false;
            dgvBloques.ReadOnly = false;
            dgvBloques.RowHeadersVisible = false;
            dgvBloques.AutoGenerateColumns = true;

            _eventosInicializados = true;
        }

        private DataTable PrepararTablaParaGrid(DataTable origen)
        {
            if (origen == null)
                return null;

            DataTable dt = origen.Copy();

            if (dt.Columns.Contains("RowVersion"))
                dt.Columns.Remove("RowVersion");

            return dt;
        }

        private void EnsureSelectionColumn()
        {
            if (dgvBloques.Columns.Contains(COL_SEL))
                return;

            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
            col.Name = COL_SEL;
            col.HeaderText = string.Empty;
            col.Width = 28;
            col.ReadOnly = false;

            dgvBloques.Columns.Insert(0, col);
        }

        private void LimpiarChecks()
        {
            if (!dgvBloques.Columns.Contains(COL_SEL))
                return;

            foreach (DataGridViewRow row in dgvBloques.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[COL_SEL].Value = false;
            }
        }

        private void CargarBloques()
        {
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                DataTable dtOriginal = _nBloque.Buscar(null, texto, soloActivos, 50);
                _dt = PrepararTablaParaGrid(dtOriginal);

                dgvBloques.DataSource = null;
                dgvBloques.DataSource = _dt;

                EnsureSelectionColumn();
                LimpiarChecks();
                AjustarGrid();

                bool hay = _dt != null && _dt.Rows.Count > 0;
                lblSinResultados.Visible = !hay;
                btnConfirmar.Enabled = hay;
            }
            catch (Exception ex)
            {
                dgvBloques.DataSource = null;
                btnConfirmar.Enabled = false;
                lblSinResultados.Visible = true;

                KryptonMessageBox.Show(
                    this,
                    "Error al cargar bloques: " + ex.Message,
                    "Buscar Bloque",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        private void AjustarGrid()
        {
            if (dgvBloques.DataSource == null)
                return;

            if (dgvBloques.Columns.Contains(COL_SEL))
            {
                dgvBloques.Columns[COL_SEL].DisplayIndex = 0;
                dgvBloques.Columns[COL_SEL].Width = 28;
            }

            if (dgvBloques.Columns.Contains("Id"))
                dgvBloques.Columns["Id"].Visible = false;

            if (dgvBloques.Columns.Contains("CondominioId"))
                dgvBloques.Columns["CondominioId"].Visible = false;

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

            foreach (DataGridViewColumn col in dgvBloques.Columns)
            {
                if (col.Name == COL_SEL)
                    continue;

                col.ReadOnly = true;
            }

            dgvBloques.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBloques.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvBloques.AllowUserToResizeRows = false;
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

        private void dgvBloques_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvBloques.IsCurrentCellDirty)
                dgvBloques.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvBloques_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (!dgvBloques.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvBloques.Columns[COL_SEL].Index)
                return;

            DataGridViewCell celda = dgvBloques.Rows[e.RowIndex].Cells[COL_SEL];
            bool actual = Convert.ToBoolean(celda.Value ?? false);
            celda.Value = !actual;
        }

        private void dgvBloques_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (!dgvBloques.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvBloques.Columns[COL_SEL].Index)
                return;

            bool marcado = Convert.ToBoolean(dgvBloques.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
            if (!marcado)
                return;

            for (int i = 0; i < dgvBloques.Rows.Count; i++)
            {
                if (i == e.RowIndex)
                    continue;

                if (dgvBloques.Rows[i].IsNewRow)
                    continue;

                dgvBloques.Rows[i].Cells[COL_SEL].Value = false;
            }
        }

        private DataRowView GetMarcado()
        {
            if (!dgvBloques.Columns.Contains(COL_SEL))
                return null;

            foreach (DataGridViewRow row in dgvBloques.Rows)
            {
                if (row.IsNewRow)
                    continue;

                bool marcado = Convert.ToBoolean(row.Cells[COL_SEL].Value ?? false);
                if (!marcado)
                    continue;

                return row.DataBoundItem as DataRowView;
            }

            return null;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            DataRowView rv = GetMarcado();
            if (rv == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Marque un bloque para confirmar.",
                    "Buscar Bloque",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            if (!rv.Row.Table.Columns.Contains("Id") || rv["Id"] == DBNull.Value)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el ID del bloque.",
                    "Buscar Bloque",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            BloqueIdSeleccionado = Convert.ToInt32(rv["Id"]);
            BloqueIdentificadorSeleccionado =
                rv.Row.Table.Columns.Contains("Identificador")
                    ? Convert.ToString(rv["Identificador"]) ?? string.Empty
                    : string.Empty;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void dgvBloques_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}