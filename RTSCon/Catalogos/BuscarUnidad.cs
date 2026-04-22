using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Data;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class BuscarUnidad : KryptonForm
    {
        private const string COL_SEL = "__sel";

        public int SelectedId { get; private set; }
        public string SelectedTexto { get; private set; } = string.Empty;

        private readonly NUnidad _negocio;
        private DataTable _dt;
        private bool _eventosInicializados;

        public BuscarUnidad()
        {
            InitializeComponent();

            _negocio = new NUnidad(new DUnidad(Conexion.CadenaConexion));

            Load -= BuscarUnidad_Load;
            Load += BuscarUnidad_Load;
        }

        private void BuscarUnidad_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            chkSoloActivos.Checked = true;
            EnsureSelectionColumn();
            Cargar();
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

            txtBuscar.TextChanged -= txtBuscar_TextChanged;
            txtBuscar.TextChanged += txtBuscar_TextChanged;

            txtBuscar.KeyDown -= txtBuscar_KeyDown;
            txtBuscar.KeyDown += txtBuscar_KeyDown;

            chkSoloActivos.CheckedChanged -= chkSoloActivos_CheckedChanged;
            chkSoloActivos.CheckedChanged += chkSoloActivos_CheckedChanged;

            dgvUnidad.CurrentCellDirtyStateChanged -= dgvUnidad_CurrentCellDirtyStateChanged;
            dgvUnidad.CurrentCellDirtyStateChanged += dgvUnidad_CurrentCellDirtyStateChanged;

            dgvUnidad.CellClick -= dgvUnidad_CellClick;
            dgvUnidad.CellClick += dgvUnidad_CellClick;

            dgvUnidad.CellValueChanged -= dgvUnidad_CellValueChanged;
            dgvUnidad.CellValueChanged += dgvUnidad_CellValueChanged;

            dgvUnidad.DataError -= dgvUnidad_DataError;
            dgvUnidad.DataError += dgvUnidad_DataError;

            dgvUnidad.MultiSelect = false;
            dgvUnidad.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnidad.AllowUserToAddRows = false;
            dgvUnidad.ReadOnly = false;
            dgvUnidad.RowHeadersVisible = false;
            dgvUnidad.AutoGenerateColumns = true;

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
            if (dgvUnidad.Columns.Contains(COL_SEL))
                return;

            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
            col.Name = COL_SEL;
            col.HeaderText = string.Empty;
            col.Width = 28;
            col.ReadOnly = false;

            dgvUnidad.Columns.Insert(0, col);
        }

        private void LimpiarChecks()
        {
            if (!dgvUnidad.Columns.Contains(COL_SEL))
                return;

            foreach (DataGridViewRow row in dgvUnidad.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[COL_SEL].Value = false;
            }
        }

        private void Cargar()
        {
            try
            {
                string filtro = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                DataTable dtOriginal = _negocio.Buscar(null, filtro, soloActivos, 50);
                _dt = PrepararTablaParaGrid(dtOriginal);

                dgvUnidad.DataSource = null;
                dgvUnidad.DataSource = _dt;

                EnsureSelectionColumn();
                LimpiarChecks();
                AjustarColumnas();

                bool hay = _dt != null && _dt.Rows.Count > 0;
                lblNoHay.Visible = !hay;
                btnConfirmar.Enabled = hay;
            }
            catch (Exception ex)
            {
                dgvUnidad.DataSource = null;
                btnConfirmar.Enabled = false;
                lblNoHay.Visible = true;

                KryptonMessageBox.Show(
                    this,
                    "Error al cargar unidades: " + ex.Message,
                    "Buscar Unidad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        private void AjustarColumnas()
        {
            if (dgvUnidad.DataSource == null)
                return;

            if (dgvUnidad.Columns.Contains(COL_SEL))
            {
                dgvUnidad.Columns[COL_SEL].DisplayIndex = 0;
                dgvUnidad.Columns[COL_SEL].Width = 28;
            }

            if (dgvUnidad.Columns.Contains("Id"))
                dgvUnidad.Columns["Id"].Visible = false;

            if (dgvUnidad.Columns.Contains("BloqueId"))
                dgvUnidad.Columns["BloqueId"].Visible = false;

            if (dgvUnidad.Columns.Contains("CreatedBy"))
                dgvUnidad.Columns["CreatedBy"].Visible = false;

            if (dgvUnidad.Columns.Contains("UpdatedBy"))
                dgvUnidad.Columns["UpdatedBy"].Visible = false;

            if (dgvUnidad.Columns.Contains("UpdatedAt"))
                dgvUnidad.Columns["UpdatedAt"].Visible = false;

            if (dgvUnidad.Columns.Contains("Numero"))
                dgvUnidad.Columns["Numero"].HeaderText = "Número";

            if (dgvUnidad.Columns.Contains("Tipologia"))
                dgvUnidad.Columns["Tipologia"].HeaderText = "Tipología";

            if (dgvUnidad.Columns.Contains("Piso"))
                dgvUnidad.Columns["Piso"].HeaderText = "Piso";

            if (dgvUnidad.Columns.Contains("Metros2"))
                dgvUnidad.Columns["Metros2"].HeaderText = "M²";

            if (dgvUnidad.Columns.Contains("IsActive"))
                dgvUnidad.Columns["IsActive"].HeaderText = "Activo";

            foreach (DataGridViewColumn col in dgvUnidad.Columns)
            {
                if (col.Name == COL_SEL)
                    continue;

                col.ReadOnly = true;
            }

            dgvUnidad.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUnidad.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvUnidad.AllowUserToResizeRows = false;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            Cargar();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Cargar();
            }
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            Cargar();
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            chkSoloActivos.Checked = true;
            Cargar();
        }

        private void dgvUnidad_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvUnidad.IsCurrentCellDirty)
                dgvUnidad.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvUnidad_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (!dgvUnidad.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvUnidad.Columns[COL_SEL].Index)
                return;

            DataGridViewCell celda = dgvUnidad.Rows[e.RowIndex].Cells[COL_SEL];
            bool actual = Convert.ToBoolean(celda.Value ?? false);
            celda.Value = !actual;
        }

        private void dgvUnidad_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (!dgvUnidad.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvUnidad.Columns[COL_SEL].Index)
                return;

            bool marcado = Convert.ToBoolean(dgvUnidad.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
            if (!marcado)
                return;

            for (int i = 0; i < dgvUnidad.Rows.Count; i++)
            {
                if (i == e.RowIndex)
                    continue;

                if (dgvUnidad.Rows[i].IsNewRow)
                    continue;

                dgvUnidad.Rows[i].Cells[COL_SEL].Value = false;
            }
        }

        private DataRowView GetMarcado()
        {
            if (!dgvUnidad.Columns.Contains(COL_SEL))
                return null;

            foreach (DataGridViewRow row in dgvUnidad.Rows)
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
            DataRowView view = GetMarcado();
            if (view == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Marque una unidad para confirmar.",
                    "Buscar Unidad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            if (!view.Row.Table.Columns.Contains("Id") || view["Id"] == DBNull.Value)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el Id de la unidad.",
                    "Buscar Unidad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            SelectedId = Convert.ToInt32(view["Id"]);

            if (view.Row.Table.Columns.Contains("Numero"))
                SelectedTexto = Convert.ToString(view["Numero"]) ?? string.Empty;
            else
                SelectedTexto = Convert.ToString(view["Id"]) ?? string.Empty;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void dgvUnidad_DataError(object sender, DataGridViewDataErrorEventArgs e)
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