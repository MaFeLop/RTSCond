using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class BuscarPropietario : KryptonForm
    {
        public int SelectedId => PropietarioIdSeleccionado;
        public string SelectedUsuario { get; private set; } = string.Empty;
        public string SelectedCorreo { get; private set; } = string.Empty;
        public string SelectedDocumento { get; private set; } = string.Empty;

        public int PropietarioIdSeleccionado { get; private set; }

        private const string COL_SEL = "__sel";

        private readonly NCondominio _negocio;
        private DataTable _dt;
        private bool _eventosInicializados;

        public BuscarPropietario()
            : this(CrearNegocio())
        {
        }

        public BuscarPropietario(NCondominio negocio)
        {
            InitializeComponent();

            _negocio = negocio ?? throw new ArgumentNullException(nameof(negocio));

            Load -= BuscarPropietario_Load;
            Load += BuscarPropietario_Load;
        }

        private static NCondominio CrearNegocio()
        {
            string cn = ConfigurationManager.ConnectionStrings["RTSCond"] != null
                ? ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString
                : null;

            if (string.IsNullOrWhiteSpace(cn))
                throw new InvalidOperationException("No se encontró la cadena de conexión 'RTSCond'.");

            return new NCondominio(new DCondominio(cn));
        }

        private void BuscarPropietario_Load(object sender, EventArgs e)
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

            dgvPropietario.CurrentCellDirtyStateChanged -= dgvPropietario_CurrentCellDirtyStateChanged;
            dgvPropietario.CurrentCellDirtyStateChanged += dgvPropietario_CurrentCellDirtyStateChanged;

            dgvPropietario.CellValueChanged -= dgvPropietario_CellValueChanged;
            dgvPropietario.CellValueChanged += dgvPropietario_CellValueChanged;

            dgvPropietario.CellClick -= dgvPropietario_CellClick;
            dgvPropietario.CellClick += dgvPropietario_CellClick;

            dgvPropietario.DataError -= dgvPropietario_DataError;
            dgvPropietario.DataError += dgvPropietario_DataError;

            dgvPropietario.MultiSelect = false;
            dgvPropietario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPropietario.AllowUserToAddRows = false;
            dgvPropietario.ReadOnly = false;
            dgvPropietario.RowHeadersVisible = false;
            dgvPropietario.AutoGenerateColumns = true;

            _eventosInicializados = true;
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void EnsureSelectionColumn()
        {
            if (dgvPropietario.Columns.Contains(COL_SEL))
                return;

            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
            col.Name = COL_SEL;
            col.HeaderText = string.Empty;
            col.Width = 28;
            col.ReadOnly = false;

            dgvPropietario.Columns.Insert(0, col);
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

        private void Cargar()
        {
            try
            {
                string filtro = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                DataTable dtOriginal = _negocio.BuscarPropietarios(filtro, soloActivos, 50);
                _dt = PrepararTablaParaGrid(dtOriginal);

                dgvPropietario.DataSource = null;
                dgvPropietario.DataSource = _dt;

                EnsureSelectionColumn();
                LimpiarChecks();
                AjustarColumnas();

                bool hay = _dt != null && _dt.Rows.Count > 0;
                lblNoHay.Visible = !hay;
                btnConfirmar.Enabled = hay;
            }
            catch (Exception ex)
            {
                dgvPropietario.DataSource = null;
                btnConfirmar.Enabled = false;
                lblNoHay.Visible = true;

                KryptonMessageBox.Show(
                    this,
                    "Error al cargar propietarios: " + ex.Message,
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        private void AjustarColumnas()
        {
            if (dgvPropietario.Columns.Contains(COL_SEL))
            {
                dgvPropietario.Columns[COL_SEL].DisplayIndex = 0;
                dgvPropietario.Columns[COL_SEL].Width = 28;
            }

            if (dgvPropietario.Columns.Contains("Id"))
                dgvPropietario.Columns["Id"].Visible = false;

            if (dgvPropietario.Columns.Contains("ID_usr"))
                dgvPropietario.Columns["ID_usr"].Visible = false;

            if (dgvPropietario.Columns.Contains("ID_propietario"))
                dgvPropietario.Columns["ID_propietario"].Visible = false;

            if (dgvPropietario.Columns.Contains("Usuario"))
                dgvPropietario.Columns["Usuario"].HeaderText = "Usuario";

            if (dgvPropietario.Columns.Contains("Documento"))
                dgvPropietario.Columns["Documento"].HeaderText = "Documento";

            if (dgvPropietario.Columns.Contains("Correo"))
                dgvPropietario.Columns["Correo"].HeaderText = "Correo";

            if (dgvPropietario.Columns.Contains("estado"))
                dgvPropietario.Columns["estado"].HeaderText = "Estado";

            foreach (DataGridViewColumn col in dgvPropietario.Columns)
            {
                if (col.Name == COL_SEL)
                    continue;

                col.ReadOnly = true;
            }

            dgvPropietario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPropietario.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvPropietario.AllowUserToResizeRows = false;
        }

        private void LimpiarChecks()
        {
            if (!dgvPropietario.Columns.Contains(COL_SEL))
                return;

            foreach (DataGridViewRow row in dgvPropietario.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[COL_SEL].Value = false;
            }
        }

        private void dgvPropietario_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPropietario.IsCurrentCellDirty)
                dgvPropietario.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvPropietario_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (!dgvPropietario.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvPropietario.Columns[COL_SEL].Index)
                return;

            DataGridViewCell celda = dgvPropietario.Rows[e.RowIndex].Cells[COL_SEL];
            bool actual = Convert.ToBoolean(celda.Value ?? false);
            celda.Value = !actual;
        }

        private void dgvPropietario_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (!dgvPropietario.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvPropietario.Columns[COL_SEL].Index)
                return;

            bool marcado = Convert.ToBoolean(dgvPropietario.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
            if (!marcado)
                return;

            for (int i = 0; i < dgvPropietario.Rows.Count; i++)
            {
                if (i == e.RowIndex)
                    continue;

                if (dgvPropietario.Rows[i].IsNewRow)
                    continue;

                dgvPropietario.Rows[i].Cells[COL_SEL].Value = false;
            }
        }

        private DataRowView GetMarcado()
        {
            if (!dgvPropietario.Columns.Contains(COL_SEL))
                return null;

            foreach (DataGridViewRow row in dgvPropietario.Rows)
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
            if (_dt == null || _dt.Rows.Count == 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "No hay propietarios para seleccionar.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            DataRowView rv = GetMarcado();
            if (rv == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Marque un propietario para confirmar.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            if (rv.Row.Table.Columns.Contains("ID_propietario") &&
                rv["ID_propietario"] != DBNull.Value &&
                !string.IsNullOrWhiteSpace(Convert.ToString(rv["ID_propietario"])))
            {
                PropietarioIdSeleccionado = Convert.ToInt32(rv["ID_propietario"]);
            }
            else if (rv.Row.Table.Columns.Contains("Id") &&
                     rv["Id"] != DBNull.Value &&
                     !string.IsNullOrWhiteSpace(Convert.ToString(rv["Id"])))
            {
                PropietarioIdSeleccionado = Convert.ToInt32(rv["Id"]);
            }
            else
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el ID del propietario.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            SelectedUsuario =
                rv.Row.Table.Columns.Contains("Usuario") ? Convert.ToString(rv["Usuario"]) :
                rv.Row.Table.Columns.Contains("Nombre") ? Convert.ToString(rv["Nombre"]) :
                rv.Row.Table.Columns.Contains("NombreCompleto") ? Convert.ToString(rv["NombreCompleto"]) :
                string.Empty;

            SelectedDocumento =
                rv.Row.Table.Columns.Contains("Documento") ? Convert.ToString(rv["Documento"]) : string.Empty;

            SelectedCorreo =
                rv.Row.Table.Columns.Contains("Correo") ? Convert.ToString(rv["Correo"]) :
                rv.Row.Table.Columns.Contains("Email") ? Convert.ToString(rv["Email"]) :
                rv.Row.Table.Columns.Contains("correo") ? Convert.ToString(rv["correo"]) :
                string.Empty;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void dgvPropietario_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
    }
}