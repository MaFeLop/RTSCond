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
        private bool _eventosInicializados;

        public PropiedadRead()
        {
            InitializeComponent();

            string cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            Load -= PropiedadRead_Load;
            Load += PropiedadRead_Load;
        }

        private void PropiedadRead_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            Cargar();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            Control txtBuscar = FindControl("txtBuscar", "txtBuscarPropiedad", "txtFiltro");
            if (txtBuscar != null)
            {
                txtBuscar.TextChanged -= TxtBuscar_TextChanged;
                txtBuscar.TextChanged += TxtBuscar_TextChanged;

                txtBuscar.KeyDown -= TxtBuscar_KeyDown;
                txtBuscar.KeyDown += TxtBuscar_KeyDown;
            }

            KryptonCheckBox chkSolo = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            if (chkSolo != null)
            {
                chkSolo.CheckedChanged -= ChkSolo_CheckedChanged;
                chkSolo.CheckedChanged += ChkSolo_CheckedChanged;
            }

            KryptonButton btnCrear = FindCtrl<KryptonButton>("btnCrear");
            KryptonButton btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            KryptonButton btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            KryptonButton btnVolver = FindCtrl<KryptonButton>("btnVolver");

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

            DataGridView grid = Dgv();
            if (grid != null)
            {
                grid.CellDoubleClick -= Grid_CellDoubleClick;
                grid.CellDoubleClick += Grid_CellDoubleClick;

                grid.CellFormatting -= Grid_CellFormatting;
                grid.CellFormatting += Grid_CellFormatting;

                grid.DataError -= Grid_DataError;
                grid.DataError += Grid_DataError;

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
            using (CrearPropiedad f = new CrearPropiedad())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                    Cargar();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DataRowView row = GetRowSeleccionada();
            if (row == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Seleccione una propiedad para actualizar.",
                    "Actualizar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            int id = ObtenerIdDesdeRow(row);
            if (id <= 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el Id de la propiedad.",
                    "Actualizar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            AbrirUpdate(id);
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            DataRowView row = GetRowSeleccionada();
            if (row == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Seleccione una propiedad para desactivar.",
                    "Desactivar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            int id = ObtenerIdDesdeRow(row);
            if (id <= 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el Id de la propiedad.",
                    "Desactivar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            byte[] rowVersion = ObtenerRowVersionPorId(id);
            if (rowVersion == null || rowVersion.Length == 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo recuperar la RowVersion de la propiedad.",
                    "Desactivar Propiedad",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            string display = ConstruirDisplayPropiedad(row);
            string mensaje = "¿Deseas desactivar la propiedad " + display + "?";

            using (PropiedadConfirmarDesactivacion frm = new PropiedadConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    string editor = UserContext.Usuario;
                    string password = frm.Password;

                    DAuth dAuth = new DAuth(Conexion.CadenaConexion);
                    NAuth nAuth = new NAuth(dAuth);

                    int usuarioId = UserContext.UsuarioAuthId;
                    bool ok = nAuth.ValidarPassword(usuarioId, password);

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

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }

        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (string n in names)
            {
                T c = Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null)
                    return c;
            }

            return null;
        }

        private Control FindControl(params string[] names)
        {
            foreach (string n in names)
            {
                Control c = Controls.Find(n, true).FirstOrDefault();
                if (c != null)
                    return c;
            }

            return null;
        }

        private string GetControlText(params string[] names)
        {
            Control c = FindControl(names);
            return c != null ? (c.Text ?? string.Empty).Trim() : string.Empty;
        }

        private DataGridView Dgv()
        {
            return FindCtrl<DataGridView>("dgvPropiedad", "dgvPropiedades", "gridPropiedad", "grid");
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

        private DataRowView GetRowSeleccionada()
        {
            DataGridView grid = Dgv();
            if (grid == null)
                return null;

            if (grid.SelectedRows != null && grid.SelectedRows.Count > 0)
                return grid.SelectedRows[0].DataBoundItem as DataRowView;

            if (grid.CurrentRow != null)
                return grid.CurrentRow.DataBoundItem as DataRowView;

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

        private byte[] ObtenerRowVersionPorId(int id)
        {
            DataRow row = _neg.PorId(id);
            if (row == null)
                return null;

            if (!row.Table.Columns.Contains("RowVersion"))
                return null;

            if (row["RowVersion"] == DBNull.Value)
                return null;

            return (byte[])row["RowVersion"];
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

            return nombre + " - " + propietario + " - Unidad " + unidad;
        }

        private void AbrirUpdate(int id)
        {
            using (UpdatePropiedad f = new UpdatePropiedad(id))
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                    Cargar();
            }
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridView grid = Dgv();
            if (grid == null)
                return;

            DataRowView row = grid.Rows[e.RowIndex].DataBoundItem as DataRowView;
            int id = ObtenerIdDesdeRow(row);

            if (id <= 0)
                return;

            AbrirUpdate(id);
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = Dgv();
            if (grid == null || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string columnName = grid.Columns[e.ColumnIndex].Name;

            if ((columnName == "IsActive" || columnName == "EsTitularPrincipal") &&
                e.Value != null &&
                e.Value != DBNull.Value)
            {
                e.Value = Convert.ToBoolean(e.Value);
                e.FormattingApplied = false;
            }
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void AjustarGrid()
        {
            DataGridView grid = Dgv();
            if (grid == null)
                return;

            if (grid.Columns.Contains("Id"))
                grid.Columns["Id"].Visible = false;

            if (grid.Columns.Contains("PropietarioId"))
                grid.Columns["PropietarioId"].Visible = false;

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
                grid.Columns["EsTitularPrincipal"].HeaderText = "Titular Principal";

            if (grid.Columns.Contains("IsActive"))
                grid.Columns["IsActive"].HeaderText = "Activo";

            if (grid.Columns.Contains("CreatedAt"))
                grid.Columns["CreatedAt"].HeaderText = "Registrado";

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.ReadOnly = true;
        }

        private void Cargar()
        {
            KryptonCheckBox chk = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            bool soloActivos = chk != null && chk.Checked;

            string buscar = GetControlText("txtBuscar", "txtBuscarPropiedad", "txtFiltro");

            int? ownerId = null;
            int total;

            DataTable dtOriginal = _neg.Listar(buscar, soloActivos, _page, _pageSize, out total, ownerId);
            DataTable dt = PrepararTablaParaGrid(dtOriginal);

            DataGridView grid = Dgv();
            if (grid != null)
            {
                grid.AutoGenerateColumns = true;
                grid.DataSource = null;
                grid.DataSource = dt;
                AjustarGrid();

                if (grid.Rows.Count > 0)
                {
                    grid.ClearSelection();
                    grid.CurrentCell = null;
                }
            }

            Control lblTotal = FindControl("lblTotal");
            Control lblPagina = FindControl("lblPagina");

            if (lblTotal != null)
                lblTotal.Text = "Total: " + total;

            if (lblPagina != null)
                lblPagina.Text = "Página: " + _page;
        }
    }
}