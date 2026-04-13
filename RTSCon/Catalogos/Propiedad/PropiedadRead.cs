using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class PropiedadRead : KryptonForm
    {
        private readonly NPropiedad _neg;
        private int _page = 1;
        private const int _pageSize = 20;

        private enum ModoAccion
        {
            Ninguno,
            Editar,
            Desactivar
        }

        private ModoAccion _modo = ModoAccion.Ninguno;
        private const string COL_SEL = "__sel";
        private bool _eventosInicializados;

        public PropiedadRead()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            Load -= PropiedadRead_Load;
            Load += PropiedadRead_Load;
        }

        private void PropiedadRead_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            EnsureSelectionColumn();
            SalirModoSeleccion();
            Cargar();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            var txtBuscar = FindCtrl<TextBox>("txtBuscar", "txtBuscarPropiedad", "txtFiltro");
            if (txtBuscar != null)
            {
                txtBuscar.TextChanged -= TxtBuscar_TextChanged;
                txtBuscar.TextChanged += TxtBuscar_TextChanged;

                txtBuscar.KeyDown -= TxtBuscar_KeyDown;
                txtBuscar.KeyDown += TxtBuscar_KeyDown;
            }

            var chkSolo = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            if (chkSolo != null)
            {
                chkSolo.CheckedChanged -= ChkSolo_CheckedChanged;
                chkSolo.CheckedChanged += ChkSolo_CheckedChanged;
            }

            var btnCrear = FindCtrl<KryptonButton>("btnCrear");
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            var btnVolver = FindCtrl<KryptonButton>("btnVolver");

            if (btnCrear != null)
            {
                btnCrear.Click -= btnCrear_Click;
                btnCrear.Click += btnCrear_Click;
            }

            if (btnUpdate != null)
            {
                btnUpdate.Click -= btnUpdate_Click;
                btnUpdate.Click += btnUpdate_Click;
            }

            if (btnDesactivar != null)
            {
                btnDesactivar.Click -= btnDesactivar_Click;
                btnDesactivar.Click += btnDesactivar_Click;
            }

            if (btnVolver != null)
            {
                btnVolver.Click -= btnVolver_Click;
                btnVolver.Click += btnVolver_Click;
            }

            var grid = Dgv();
            if (grid != null)
            {
                grid.CurrentCellDirtyStateChanged -= Grid_CurrentCellDirtyStateChanged;
                grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;

                grid.CellContentClick -= Grid_CellContentClick;
                grid.CellContentClick += Grid_CellContentClick;

                grid.CellDoubleClick -= Grid_CellDoubleClick;
                grid.CellDoubleClick += Grid_CellDoubleClick;

                grid.CellFormatting -= Grid_CellFormatting;
                grid.CellFormatting += Grid_CellFormatting;

                grid.MultiSelect = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.ReadOnly = true;
                grid.AutoGenerateColumns = true;
                grid.AllowUserToAddRows = false;
                grid.RowHeadersVisible = false;
            }

            _eventosInicializados = true;
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            _page = 1;
            Cargar();
        }

        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                _page = 1;
                Cargar();
            }
        }

        private void ChkSolo_CheckedChanged(object sender, EventArgs e)
        {
            _page = 1;
            Cargar();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (_modo != ModoAccion.Ninguno)
                return;

            using (var f = new CrearPropiedad())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                    Cargar();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var btnCrear = FindCtrl<KryptonButton>("btnCrear");
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            var btnVolver = FindCtrl<KryptonButton>("btnVolver");

            if (_modo == ModoAccion.Ninguno)
            {
                EntrarModoSeleccion(ModoAccion.Editar, btnUpdate, btnDesactivar, btnCrear, btnVolver);
                return;
            }

            if (_modo != ModoAccion.Editar)
                return;

            var row = GetRowMarcado();
            if (row == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Marque una propiedad (checkbox) para actualizar.",
                    "Actualizar",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            int id = ObtenerIdDesdeRow(row);
            if (id <= 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el Id.",
                    "Actualizar",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);
            AbrirUpdate(id);
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            var btnCrear = FindCtrl<KryptonButton>("btnCrear");
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            var btnVolver = FindCtrl<KryptonButton>("btnVolver");

            if (_modo == ModoAccion.Ninguno)
            {
                EntrarModoSeleccion(ModoAccion.Desactivar, btnUpdate, btnDesactivar, btnCrear, btnVolver);
                return;
            }

            if (_modo != ModoAccion.Desactivar)
                return;

            var row = GetRowMarcado();
            if (row == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Marque una propiedad (checkbox) para desactivar.",
                    "Desactivar",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            int id = ObtenerIdDesdeRow(row);
            if (id <= 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el Id.",
                    "Desactivar",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            byte[] rowVersion = null;
            if (row.Row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                rowVersion = (byte[])row["RowVersion"];

            string display = ConstruirDisplayPropiedad(row);

            SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);

            string mensaje = $"¿Deseas desactivar la propiedad {display}?";

            using (var frm = new PropiedadConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string editor = UserContext.Usuario;
                        string password = frm.Password;

                        var dAuth = new DAuth(Conexion.CadenaConexion);
                        var nAuth = new NAuth(dAuth);

                        int usuarioId = UserContext.UsuarioAuthId;
                        var ok = nAuth.ValidarPassword(usuarioId, password);

                        if (!ok)
                            throw new InvalidOperationException("Contraseña inválida.");

                        _neg.Desactivar(id, rowVersion, editor);
                        Cargar();
                    }
                    catch (Exception ex)
                    {
                        KryptonMessageBox.Show(
                            this,
                            "No se pudo desactivar la propiedad: " + ex.Message,
                            "Desactivar Propiedad",
                            KryptonMessageBoxButtons.OK,
                            KryptonMessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            var btnCrear = FindCtrl<KryptonButton>("btnCrear");
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            var btnVolver = FindCtrl<KryptonButton>("btnVolver");

            if (_modo != ModoAccion.Ninguno)
            {
                SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);
                return;
            }

            Close();
        }

        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (var n in names)
            {
                var c = Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null)
                    return c;
            }
            return null;
        }

        private DataGridView Dgv()
        {
            return FindCtrl<DataGridView>("dgvPropiedad", "dgvPropiedades", "gridPropiedad", "grid");
        }

        private void EnsureSelectionColumn()
        {
            var grid = Dgv();
            if (grid == null)
                return;

            if (grid.Columns.Contains(COL_SEL))
                return;

            var col = new DataGridViewCheckBoxColumn
            {
                Name = COL_SEL,
                HeaderText = string.Empty,
                Width = 28,
                ReadOnly = false,
                Visible = false
            };

            grid.Columns.Insert(0, col);
        }

        private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var grid = Dgv();
            if (grid == null)
                return;

            if (grid.IsCurrentCellDirty)
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = Dgv();
            if (grid == null)
                return;

            if (_modo == ModoAccion.Ninguno)
                return;

            if (e.RowIndex < 0)
                return;

            if (!grid.Columns.Contains(COL_SEL))
                return;

            if (grid.Columns[e.ColumnIndex].Name != COL_SEL)
                return;

            bool marcado = Convert.ToBoolean(grid.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
            if (marcado)
            {
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    if (i == e.RowIndex)
                        continue;

                    if (grid.Rows[i].IsNewRow)
                        continue;

                    grid.Rows[i].Cells[COL_SEL].Value = false;
                }
            }
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_modo != ModoAccion.Ninguno)
                return;

            if (e.RowIndex < 0)
                return;

            var row = GetRowActual(e.RowIndex);
            if (row == null)
                return;

            int id = ObtenerIdDesdeRow(row);
            if (id <= 0)
                return;

            AbrirUpdate(id);
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = Dgv();
            if (grid == null || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string columnName = grid.Columns[e.ColumnIndex].Name;

            if (columnName == "IsActive" && e.Value != null && e.Value != DBNull.Value)
            {
                e.Value = Convert.ToBoolean(e.Value);
                e.FormattingApplied = false;
            }

            if (columnName == "EsTitularPrincipal" && e.Value != null && e.Value != DBNull.Value)
            {
                e.Value = Convert.ToBoolean(e.Value);
                e.FormattingApplied = false;
            }
        }

        private void SetSelectionColumnVisible(bool visible)
        {
            var grid = Dgv();
            if (grid == null)
                return;

            if (!grid.Columns.Contains(COL_SEL))
                return;

            grid.Columns[COL_SEL].Visible = visible;

            if (visible)
            {
                grid.ReadOnly = false;
                foreach (DataGridViewColumn c in grid.Columns)
                    c.ReadOnly = true;

                grid.Columns[COL_SEL].ReadOnly = false;
            }
            else
            {
                grid.ReadOnly = true;
                foreach (DataGridViewColumn c in grid.Columns)
                    c.ReadOnly = true;
            }
        }

        private DataRowView GetRowMarcado()
        {
            var grid = Dgv();
            if (grid == null || !grid.Columns.Contains(COL_SEL))
                return null;

            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r.IsNewRow)
                    continue;

                bool marcado = Convert.ToBoolean(r.Cells[COL_SEL].Value ?? false);
                if (!marcado)
                    continue;

                return r.DataBoundItem as DataRowView;
            }

            return null;
        }

        private DataRowView GetRowActual(int rowIndex)
        {
            var grid = Dgv();
            if (grid == null)
                return null;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return null;

            return grid.Rows[rowIndex].DataBoundItem as DataRowView;
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

        private string ConstruirDisplayPropiedad(DataRowView row)
        {
            if (row == null)
                return "seleccionada";

            string nombre = row.Row.Table.Columns.Contains("Nombre") && row["Nombre"] != DBNull.Value
                ? Convert.ToString(row["Nombre"])
                : "Propiedad";

            string propietario = row.Row.Table.Columns.Contains("PropietarioNombre") && row["PropietarioNombre"] != DBNull.Value
                ? Convert.ToString(row["PropietarioNombre"])
                : "Sin propietario";

            string unidad = row.Row.Table.Columns.Contains("UnidadId") && row["UnidadId"] != DBNull.Value
                ? Convert.ToString(row["UnidadId"])
                : "?";

            return $"{nombre} - {propietario} - Unidad {unidad}";
        }

        private void AbrirUpdate(int id)
        {
            using (var f = new UpdatePropiedad())
            {
                f.Tag = id;
                if (f.ShowDialog(this) == DialogResult.OK)
                    Cargar();
            }
        }

        private void EntrarModoSeleccion(
            ModoAccion modo,
            KryptonButton btnUpdate,
            KryptonButton btnDesactivar,
            KryptonButton btnCrear,
            KryptonButton btnVolver)
        {
            _modo = modo;
            SetSelectionColumnVisible(true);

            var grid = Dgv();
            if (grid != null)
            {
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if (r.IsNewRow)
                        continue;

                    r.Cells[COL_SEL].Value = false;
                }
            }

            if (modo == ModoAccion.Editar)
            {
                if (btnUpdate != null) btnUpdate.Text = "Confirmar";
                if (btnDesactivar != null) btnDesactivar.Enabled = false;
                if (btnCrear != null) btnCrear.Enabled = false;
            }
            else if (modo == ModoAccion.Desactivar)
            {
                if (btnDesactivar != null) btnDesactivar.Text = "Confirmar";
                if (btnUpdate != null) btnUpdate.Enabled = false;
                if (btnCrear != null) btnCrear.Enabled = false;
            }

            if (btnVolver != null)
                btnVolver.Text = "Cancelar";
        }

        private void SalirModoSeleccion()
        {
            var btnCrear = FindCtrl<KryptonButton>("btnCrear");
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            var btnVolver = FindCtrl<KryptonButton>("btnVolver");

            SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);
        }

        private void SalirModoSeleccion(
            KryptonButton btnUpdate,
            KryptonButton btnDesactivar,
            KryptonButton btnCrear,
            KryptonButton btnVolver)
        {
            _modo = ModoAccion.Ninguno;

            var grid = Dgv();
            if (grid != null && grid.Columns.Contains(COL_SEL))
            {
                grid.Columns[COL_SEL].Visible = false;
                grid.ReadOnly = true;
                foreach (DataGridViewColumn c in grid.Columns)
                    c.ReadOnly = true;
            }

            if (btnUpdate != null)
            {
                btnUpdate.Text = "Update";
                btnUpdate.Enabled = true;
            }

            if (btnDesactivar != null)
            {
                btnDesactivar.Text = "Desactivar";
                btnDesactivar.Enabled = true;
            }

            if (btnCrear != null)
                btnCrear.Enabled = true;

            if (btnVolver != null)
                btnVolver.Text = "Volver";
        }

        private void AjustarGrid()
        {
            var grid = Dgv();
            if (grid == null)
                return;

            if (grid.Columns.Contains("Id"))
                grid.Columns["Id"].Visible = false;

            if (grid.Columns.Contains("PropietarioId"))
                grid.Columns["PropietarioId"].Visible = false;

            if (grid.Columns.Contains("RowVersion"))
                grid.Columns["RowVersion"].Visible = false;

            if (grid.Columns.Contains("CreatedBy"))
                grid.Columns["CreatedBy"].Visible = false;

            if (grid.Columns.Contains("UpdatedBy"))
                grid.Columns["UpdatedBy"].Visible = false;

            if (grid.Columns.Contains("UpdatedAt"))
                grid.Columns["UpdatedAt"].Visible = false;

            if (grid.Columns.Contains("Nombre"))
                grid.Columns["Nombre"].HeaderText = "Nombre";

            if (grid.Columns.Contains("UnidadId"))
                grid.Columns["UnidadId"].HeaderText = "Unidad";

            if (grid.Columns.Contains("PropietarioNombre"))
                grid.Columns["PropietarioNombre"].HeaderText = "Propietario";

            if (grid.Columns.Contains("PropietarioDocumento"))
                grid.Columns["PropietarioDocumento"].HeaderText = "Documento";

            if (grid.Columns.Contains("Correo"))
                grid.Columns["Correo"].HeaderText = "Correo";

            if (grid.Columns.Contains("FechaInicio"))
                grid.Columns["FechaInicio"].HeaderText = "Inicio";

            if (grid.Columns.Contains("FechaFin"))
                grid.Columns["FechaFin"].HeaderText = "Fin";

            if (grid.Columns.Contains("Porcentaje"))
                grid.Columns["Porcentaje"].HeaderText = "% Propiedad";

            if (grid.Columns.Contains("EsTitularPrincipal"))
            {
                grid.Columns["EsTitularPrincipal"].HeaderText = "Titular Principal";
                if (grid.Columns["EsTitularPrincipal"] is DataGridViewCheckBoxColumn chkTitular)
                    chkTitular.TrueValue = true;
            }

            if (grid.Columns.Contains("IsActive"))
            {
                grid.Columns["IsActive"].HeaderText = "Activo";
                if (grid.Columns["IsActive"] is DataGridViewCheckBoxColumn chkActivo)
                    chkActivo.TrueValue = true;
            }

            if (grid.Columns.Contains("CreatedAt"))
                grid.Columns["CreatedAt"].HeaderText = "Registrado";

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
        }

        private void Cargar()
        {
            var chk = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            bool soloActivos = chk?.Checked ?? false;

            var txtBuscar = FindCtrl<TextBox>("txtBuscar", "txtBuscarPropiedad", "txtFiltro");
            string buscar = txtBuscar?.Text?.Trim() ?? string.Empty;

            int? ownerId = null;

            int total;
            var dt = _neg.Listar(buscar, soloActivos, _page, _pageSize, out total, ownerId);

            var grid = Dgv();
            if (grid != null)
            {
                grid.AutoGenerateColumns = true;
                grid.DataSource = dt;
                AjustarGrid();
            }

            var lblTotal = FindCtrl<Label>("lblTotal");
            var lblPagina = FindCtrl<Label>("lblPagina");

            if (lblTotal != null)
                lblTotal.Text = $"Total: {total}";

            if (lblPagina != null)
                lblPagina.Text = $"Página: {_page}";

            SetSelectionColumnVisible(_modo != ModoAccion.Ninguno);
        }
    }
}