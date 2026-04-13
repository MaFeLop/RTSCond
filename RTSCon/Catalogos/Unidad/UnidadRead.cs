using System;
using System.Data;
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

        private enum ModoAccion { Ninguno, Editar, Desactivar }
        private ModoAccion _modo = ModoAccion.Ninguno;

        private const string COL_SEL = "__sel";

        public UnidadRead() : this(0)
        {
        }

        public UnidadRead(int bloqueId)
        {
            InitializeComponent();

            _bloqueId = bloqueId;

            var dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            dgvUnidades.AutoGenerateColumns = true;

            Load -= UnidadRead_Load;
            Load += UnidadRead_Load;
        }

        private void UnidadRead_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            EnsureSelectionColumn();

            chkSoloActivos.Checked = true;
            CargarBloque();
            CargarUnidades();
            SalirModoSeleccion();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            dgvUnidades.CurrentCellDirtyStateChanged -= dgvUnidades_CurrentCellDirtyStateChanged;
            dgvUnidades.CurrentCellDirtyStateChanged += dgvUnidades_CurrentCellDirtyStateChanged;

            dgvUnidades.CellContentClick -= dgvUnidades_CellContentClick;
            dgvUnidades.CellContentClick += dgvUnidades_CellContentClick;

            dgvUnidades.CellDoubleClick -= dgvUnidades_CellDoubleClick;
            dgvUnidades.CellDoubleClick += dgvUnidades_CellDoubleClick;

            dgvUnidades.MultiSelect = false;
            dgvUnidades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnidades.AllowUserToAddRows = false;
            dgvUnidades.RowHeadersVisible = false;

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

        private void CargarBloque()
        {
            try
            {
                if (_bloqueId <= 0)
                    return;

                var rowBloque = _nBloque.PorId(_bloqueId);
                if (rowBloque != null)
                    Text = $"Unidades del bloque {rowBloque["Identificador"]}";
            }
            catch
            {
            }
        }

        private void CargarUnidades()
        {
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                int? bloqueFiltro = _bloqueId > 0 ? (int?)_bloqueId : null;

                DataTable dt = _nUnidad.Buscar(bloqueFiltro, texto, soloActivos, 50);
                dgvUnidades.DataSource = dt;
                AjustarGrid();

                lblTotal.Text = $"Total: {(dt?.Rows.Count ?? 0)}";

                SetSelectionColumnVisible(_modo != ModoAccion.Ninguno);
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

            if (dgvUnidades.Columns.Contains("RowVersion"))
                dgvUnidades.Columns["RowVersion"].Visible = false;

            if (dgvUnidades.Columns.Contains("CreatedBy"))
                dgvUnidades.Columns["CreatedBy"].Visible = false;

            if (dgvUnidades.Columns.Contains("UpdatedBy"))
                dgvUnidades.Columns["UpdatedBy"].Visible = false;

            if (dgvUnidades.Columns.Contains("UpdatedAt"))
                dgvUnidades.Columns["UpdatedAt"].Visible = false;

            if (dgvUnidades.Columns.Contains("Numero"))
                dgvUnidades.Columns["Numero"].HeaderText = "Número";

            if (dgvUnidades.Columns.Contains("Tipo"))
                dgvUnidades.Columns["Tipo"].HeaderText = "Tipo";

            if (dgvUnidades.Columns.Contains("Piso"))
                dgvUnidades.Columns["Piso"].HeaderText = "Piso";

            if (dgvUnidades.Columns.Contains("MetrosCuadrados"))
                dgvUnidades.Columns["MetrosCuadrados"].HeaderText = "M²";

            if (dgvUnidades.Columns.Contains("IsActive"))
                dgvUnidades.Columns["IsActive"].HeaderText = "Activo";

            dgvUnidades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUnidades.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvUnidades.AllowUserToResizeRows = false;
        }

        private DataRow FilaSeleccionada()
        {
            if (dgvUnidades.CurrentRow == null)
                return null;

            var view = dgvUnidades.CurrentRow.DataBoundItem as DataRowView;
            return view?.Row;
        }

        private void EnsureSelectionColumn()
        {
            if (dgvUnidades.Columns.Contains(COL_SEL))
                return;

            var col = new DataGridViewCheckBoxColumn
            {
                Name = COL_SEL,
                HeaderText = "",
                Width = 28,
                ReadOnly = false,
                Visible = false
            };

            dgvUnidades.Columns.Insert(0, col);
        }

        private void SetSelectionColumnVisible(bool visible)
        {
            if (!dgvUnidades.Columns.Contains(COL_SEL))
                return;

            dgvUnidades.Columns[COL_SEL].Visible = visible;
        }

        private void dgvUnidades_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvUnidades.IsCurrentCellDirty)
                dgvUnidades.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvUnidades_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_modo == ModoAccion.Ninguno)
                return;

            if (e.RowIndex < 0)
                return;

            if (dgvUnidades.Columns[e.ColumnIndex].Name != COL_SEL)
                return;

            var marcado = Convert.ToBoolean(dgvUnidades.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
            if (marcado)
            {
                for (int i = 0; i < dgvUnidades.Rows.Count; i++)
                {
                    if (i == e.RowIndex)
                        continue;

                    if (dgvUnidades.Rows[i].IsNewRow)
                        continue;

                    dgvUnidades.Rows[i].Cells[COL_SEL].Value = false;
                }
            }
        }

        private DataRow GetRowMarcado()
        {
            if (_modo == ModoAccion.Ninguno)
                return null;

            foreach (DataGridViewRow gridRow in dgvUnidades.Rows)
            {
                if (gridRow.IsNewRow)
                    continue;

                bool marcado = Convert.ToBoolean(gridRow.Cells[COL_SEL].Value ?? false);
                if (!marcado)
                    continue;

                var view = gridRow.DataBoundItem as DataRowView;
                return view?.Row;
            }

            return null;
        }

        private void EntrarModoSeleccion(ModoAccion modo)
        {
            _modo = modo;
            SetSelectionColumnVisible(true);

            foreach (DataGridViewRow r in dgvUnidades.Rows)
            {
                if (r.IsNewRow)
                    continue;

                r.Cells[COL_SEL].Value = false;
            }

            if (modo == ModoAccion.Editar)
            {
                btnUpdate.Text = "Confirmar";
                btnDesactivar.Enabled = false;
                btnCrear.Enabled = false;
            }
            else if (modo == ModoAccion.Desactivar)
            {
                btnDesactivar.Text = "Confirmar";
                btnUpdate.Enabled = false;
                btnCrear.Enabled = false;
            }

            btnVolver.Visible = true;
        }

        private void SalirModoSeleccion()
        {
            _modo = ModoAccion.Ninguno;
            SetSelectionColumnVisible(false);

            btnUpdate.Text = "Update";
            btnDesactivar.Text = "Desactivar";
            btnCrear.Enabled = true;
            btnUpdate.Enabled = true;
            btnDesactivar.Enabled = true;
            btnVolver.Visible = false;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (_modo != ModoAccion.Ninguno)
                return;

            if (_bloqueId <= 0)
            {
                using (var frmBuscar = new BuscarBloque())
                {
                    if (frmBuscar.ShowDialog(this) != DialogResult.OK)
                        return;

                    _bloqueId = frmBuscar.BloqueIdSeleccionado;
                    CargarBloque();
                }
            }

            using (var frm = new CrearUnidad(_bloqueId))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (_modo == ModoAccion.Ninguno)
            {
                EntrarModoSeleccion(ModoAccion.Editar);
                return;
            }

            if (_modo != ModoAccion.Editar)
                return;

            var row = GetRowMarcado();
            if (row == null)
            {
                MessageBox.Show(
                    "Marca una unidad (checkbox) para actualizar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(row["Id"]);

            SalirModoSeleccion();

            using (var frm = new UpdateUnidad(id))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void dgvUnidades_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && _modo == ModoAccion.Ninguno)
                btnEditar_Click(sender, EventArgs.Empty);
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (_modo == ModoAccion.Ninguno)
            {
                EntrarModoSeleccion(ModoAccion.Desactivar);
                return;
            }

            if (_modo != ModoAccion.Desactivar)
                return;

            var row = GetRowMarcado();
            if (row == null)
            {
                MessageBox.Show(
                    "Marca una unidad (checkbox) para desactivar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(row["Id"]);
            byte[] rowVersion = (byte[])row["RowVersion"];
            string numero = Convert.ToString(row["Numero"]) ?? string.Empty;

            SalirModoSeleccion();

            string mensaje = $"¿Deseas desactivar la unidad '{numero}'?";

            using (var frm = new UnidadConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        int usuarioId = UserContext.UsuarioAuthId;
                        string usuarioLogin = UserContext.Usuario;
                        string password = frm.Password;

                        var dAuth = new DAuth(Conexion.CadenaConexion);
                        var nAuth = new NAuth(dAuth);

                        var ok = nAuth.ValidarPassword(usuarioId, password);
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
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (_modo != ModoAccion.Ninguno)
            {
                SalirModoSeleccion();
                return;
            }

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
            CargarUnidades();
        }
    }
}