using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class UnidadRead : Form
    {
        private int _bloqueId;
        private readonly NUnidad _nUnidad;
        private readonly NBloque _nBloque;
        private bool _eventosInicializados;
        private bool _inicializando;
        private DataTable _dtDatos;

        public UnidadRead()
            : this(0)
        {
        }

        public UnidadRead(int bloqueId)
        {
            InitializeComponent();

            _bloqueId = bloqueId;

            DUnidad dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            DBloque dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            dgvUnidades.AutoGenerateColumns = true;

            Load -= UnidadRead_Load;
            Load += UnidadRead_Load;
        }

        private void UnidadRead_Load(object sender, EventArgs e)
        {
            _inicializando = true;

            try
            {
                InicializarEventosUnaSolaVez();
                ConfigurarVista();
                CargarBloque();

                if (!chkSoloActivos.Checked)
                    chkSoloActivos.Checked = true;
            }
            finally
            {
                _inicializando = false;
            }

            CargarUnidades();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            dgvUnidades.CellDoubleClick -= dgvUnidades_CellDoubleClick;
            dgvUnidades.CellDoubleClick += dgvUnidades_CellDoubleClick;

            dgvUnidades.CellFormatting -= dgvUnidades_CellFormatting;
            dgvUnidades.CellFormatting += dgvUnidades_CellFormatting;

            dgvUnidades.DataError -= dgvUnidades_DataError;
            dgvUnidades.DataError += dgvUnidades_DataError;

            dgvUnidades.MultiSelect = false;
            dgvUnidades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnidades.AllowUserToAddRows = false;
            dgvUnidades.RowHeadersVisible = false;
            dgvUnidades.ReadOnly = true;

            btnCrear.Click -= btnNuevo_Click;
            btnCrear.Click += btnNuevo_Click;

            btnUpdate.Click -= btnEditar_Click;
            btnUpdate.Click += btnEditar_Click;

            btnDesactivar.Click -= btnDesactivar_Click;
            btnDesactivar.Click += btnDesactivar_Click;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            btnLimpiarFiltros.Click -= btnLimpiarFiltros_Click;
            btnLimpiarFiltros.Click += btnLimpiarFiltros_Click;

            txtBuscar.TextChanged -= txtBuscar_TextChanged;
            txtBuscar.TextChanged += txtBuscar_TextChanged;

            txtBuscar.KeyDown -= txtBuscar_KeyDown;
            txtBuscar.KeyDown += txtBuscar_KeyDown;

            chkSoloActivos.CheckedChanged -= chkSoloActivos_CheckedChanged;
            chkSoloActivos.CheckedChanged += chkSoloActivos_CheckedChanged;

            _eventosInicializados = true;
        }

        private void ConfigurarVista()
        {
            dgvUnidades.ReadOnly = true;
            dgvUnidades.MultiSelect = false;
            dgvUnidades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnidades.AllowUserToAddRows = false;
            dgvUnidades.RowHeadersVisible = false;
            dgvUnidades.AutoGenerateColumns = true;

            btnVolver.Visible = true;
            btnVolver.Text = "Volver";

            if (btnVolver.Left <= btnDesactivar.Right)
            {
                btnVolver.Location = new Point(btnDesactivar.Right + 25, btnDesactivar.Top);
            }
        }

        private void CargarBloque()
        {
            try
            {
                if (_bloqueId <= 0)
                {
                    Text = "Unidades";
                    return;
                }

                DataRow rowBloque = _nBloque.PorId(_bloqueId);
                if (rowBloque != null)
                    Text = string.Format("Unidades del bloque {0}", rowBloque["Identificador"]);
                else
                    Text = "Unidades";
            }
            catch
            {
                Text = "Unidades";
            }
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

        private void CargarUnidades()
        {
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;
                int? bloqueFiltro = _bloqueId > 0 ? (int?)_bloqueId : null;

                DataTable dtOriginal = _nUnidad.Buscar(bloqueFiltro, texto, soloActivos, 50);
                _dtDatos = dtOriginal;

                DataTable dtGrid = PrepararTablaParaGrid(dtOriginal);

                dgvUnidades.DataSource = null;
                dgvUnidades.DataSource = dtGrid;

                AjustarGrid();

                lblTotal.Text = string.Format("Total: {0}", dtGrid != null ? dtGrid.Rows.Count : 0);

                if (dgvUnidades.Rows.Count > 0)
                    dgvUnidades.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar unidades: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AjustarGrid()
        {
            if (dgvUnidades.DataSource == null)
                return;

            if (dgvUnidades.Columns.Contains("Id"))
                dgvUnidades.Columns["Id"].Visible = false;

            if (dgvUnidades.Columns.Contains("BloqueId"))
                dgvUnidades.Columns["BloqueId"].Visible = false;

            if (dgvUnidades.Columns.Contains("CreatedBy"))
                dgvUnidades.Columns["CreatedBy"].Visible = false;

            if (dgvUnidades.Columns.Contains("UpdatedBy"))
                dgvUnidades.Columns["UpdatedBy"].Visible = false;

            if (dgvUnidades.Columns.Contains("UpdatedAt"))
                dgvUnidades.Columns["UpdatedAt"].Visible = false;

            if (dgvUnidades.Columns.Contains("CreatedAt"))
                dgvUnidades.Columns["CreatedAt"].Visible = false;

            if (dgvUnidades.Columns.Contains("Numero"))
                dgvUnidades.Columns["Numero"].HeaderText = "Número";

            if (dgvUnidades.Columns.Contains("Piso"))
                dgvUnidades.Columns["Piso"].HeaderText = "Piso";

            if (dgvUnidades.Columns.Contains("Tipologia"))
                dgvUnidades.Columns["Tipologia"].HeaderText = "Tipología";

            if (dgvUnidades.Columns.Contains("Metros2"))
                dgvUnidades.Columns["Metros2"].HeaderText = "Metros2";

            if (dgvUnidades.Columns.Contains("Estacionamiento"))
                dgvUnidades.Columns["Estacionamiento"].HeaderText = "Estacionamiento";

            if (dgvUnidades.Columns.Contains("Amueblada"))
                dgvUnidades.Columns["Amueblada"].HeaderText = "Amueblada";

            if (dgvUnidades.Columns.Contains("CantidadMuebles"))
                dgvUnidades.Columns["CantidadMuebles"].HeaderText = "CantidadMuebles";

            if (dgvUnidades.Columns.Contains("CuotaMantenimientoEspecifica"))
                dgvUnidades.Columns["CuotaMantenimientoEspecifica"].HeaderText = "CuotaMantenimiento";

            if (dgvUnidades.Columns.Contains("Observaciones"))
                dgvUnidades.Columns["Observaciones"].HeaderText = "Observaciones";

            if (dgvUnidades.Columns.Contains("IsActive"))
                dgvUnidades.Columns["IsActive"].HeaderText = "Activo";

            dgvUnidades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUnidades.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvUnidades.AllowUserToResizeRows = false;
            dgvUnidades.ReadOnly = true;
        }

        private DataRowView GetRowSeleccionada()
        {
            if (dgvUnidades.SelectedRows != null && dgvUnidades.SelectedRows.Count > 0)
                return dgvUnidades.SelectedRows[0].DataBoundItem as DataRowView;

            if (dgvUnidades.CurrentRow != null)
                return dgvUnidades.CurrentRow.DataBoundItem as DataRowView;

            return null;
        }

        private int ObtenerIdDesdeRow(DataRowView row)
        {
            if (row == null)
                return 0;

            if (!row.Row.Table.Columns.Contains("Id"))
                return 0;

            if (row["Id"] == DBNull.Value)
                return 0;

            return Convert.ToInt32(row["Id"]);
        }

        private byte[] ObtenerRowVersionDesdeId(int id)
        {
            if (_dtDatos == null)
                return null;

            DataRow[] rows = _dtDatos.Select("Id = " + id);
            if (rows == null || rows.Length == 0)
                return null;

            if (!rows[0].Table.Columns.Contains("RowVersion"))
                return null;

            if (rows[0]["RowVersion"] == DBNull.Value)
                return null;

            return (byte[])rows[0]["RowVersion"];
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using (CrearUnidad frm = new CrearUnidad(_bloqueId))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            DataRowView row = GetRowSeleccionada();
            if (row == null)
            {
                MessageBox.Show(
                    "Selecciona una unidad para actualizar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int id = ObtenerIdDesdeRow(row);
            if (id <= 0)
            {
                MessageBox.Show(
                    "No se pudo obtener el Id de la unidad.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using (UpdateUnidad frm = new UpdateUnidad(id))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void dgvUnidades_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataRowView row = dgvUnidades.Rows[e.RowIndex].DataBoundItem as DataRowView;
            int id = ObtenerIdDesdeRow(row);

            if (id <= 0)
                return;

            using (UpdateUnidad frm = new UpdateUnidad(id))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            DataRowView row = GetRowSeleccionada();
            if (row == null)
            {
                MessageBox.Show(
                    "Selecciona una unidad para desactivar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int id = ObtenerIdDesdeRow(row);
            if (id <= 0)
            {
                MessageBox.Show(
                    "No se pudo obtener el Id de la unidad.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            byte[] rowVersion = ObtenerRowVersionDesdeId(id);
            if (rowVersion == null || rowVersion.Length == 0)
            {
                MessageBox.Show(
                    "No se pudo recuperar la RowVersion de la unidad.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string numero = row.Row.Table.Columns.Contains("Numero") && row["Numero"] != DBNull.Value
                ? Convert.ToString(row["Numero"])
                : string.Empty;

            string mensaje = string.Format("¿Deseas desactivar la unidad '{0}'?", numero);

            using (UnidadConfirmarDesactivacion frm = new UnidadConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    int usuarioId = UserContext.UsuarioAuthId;
                    if (usuarioId <= 0)
                        throw new InvalidOperationException("La sesión actual no tiene un Id de usuario válido.");

                    string usuarioLogin = UserContext.Usuario;
                    string password = frm.Password;

                    DAuth dAuth = new DAuth(Conexion.CadenaConexion);
                    NAuth nAuth = new NAuth(dAuth);

                    bool ok = nAuth.ValidarPassword(usuarioId, password);
                    if (!ok)
                        throw new InvalidOperationException("Contraseña inválida.");

                    _nUnidad.Desactivar(id, rowVersion, usuarioLogin);
                    CargarUnidades();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "No se pudo desactivar la unidad: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            chkSoloActivos.Checked = true;
            CargarUnidades();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (_inicializando)
                return;

            CargarUnidades();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CargarUnidades();
            }
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            if (_inicializando)
                return;

            CargarUnidades();
        }

        private void dgvUnidades_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string columnName = dgvUnidades.Columns[e.ColumnIndex].Name;

            if ((columnName == "IsActive" || columnName == "Amueblada") &&
                e.Value != null &&
                e.Value != DBNull.Value)
            {
                e.Value = Convert.ToBoolean(e.Value);
                e.FormattingApplied = false;
            }
        }

        private void dgvUnidades_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
    }
}