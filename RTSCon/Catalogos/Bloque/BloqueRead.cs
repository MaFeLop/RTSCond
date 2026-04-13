using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class BloqueRead : Form
    {
        private int _condominioId;
        private readonly NBloque _nBloque;
        private bool _eventosInicializados;

        public BloqueRead() : this(0)
        {
        }

        public BloqueRead(int condominioId)
        {
            InitializeComponent();

            _condominioId = condominioId;

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            dgvBloques.AutoGenerateColumns = true;

            Load -= BloqueRead_Load;
            Load += BloqueRead_Load;
        }

        private void BloqueRead_Load(object sender, EventArgs e)
        {
            InicializarEventosUnaSolaVez();
            chkSoloActivos.Checked = true;
            CargarBloques();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            btnCrear.Click -= btnNuevo_Click;
            btnCrear.Click += btnNuevo_Click;

            btnUpdate.Click -= btnEditar_Click;
            btnUpdate.Click += btnEditar_Click;

            btnDesactivar.Click -= btnDesactivar_Click;
            btnDesactivar.Click += btnDesactivar_Click;

            btnLimpiarFiltros.Click -= btnLimpiarFiltros_Click;
            btnLimpiarFiltros.Click += btnLimpiarFiltros_Click;

            txtBuscar.TextChanged -= txtBuscar_TextChanged;
            txtBuscar.TextChanged += txtBuscar_TextChanged;

            txtBuscar.KeyDown -= txtBuscar_KeyDown;
            txtBuscar.KeyDown += txtBuscar_KeyDown;

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
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                int? condominioFiltro = _condominioId > 0
                    ? (int?)_condominioId
                    : null;

                DataTable dt = _nBloque.Buscar(condominioFiltro, texto, soloActivos, 50);
                dgvBloques.DataSource = dt;

                AjustarGrid();

                lblTotal.Text = $"Total: {(dt?.Rows.Count ?? 0)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar bloques: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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

        private DataRow FilaSeleccionada()
        {
            if (dgvBloques.CurrentRow == null)
                return null;

            var view = dgvBloques.CurrentRow.DataBoundItem as DataRowView;
            return view?.Row;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (_condominioId <= 0)
            {
                using (var frmBuscar = new BuscarCondominio())
                {
                    if (frmBuscar.ShowDialog(this) != DialogResult.OK)
                        return;

                    _condominioId = frmBuscar.CondominioIdSeleccionado;
                }
            }

            using (var frm = new CrearBloque(_condominioId))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarBloques();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var row = FilaSeleccionada();

            if (row == null)
            {
                MessageBox.Show(
                    "Selecciona un bloque primero.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(row["Id"]);

            using (var frm = new UpdateBloque(id))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarBloques();
            }
        }

        private void dgvBloques_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnEditar_Click(sender, EventArgs.Empty);
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            var row = FilaSeleccionada();

            if (row == null)
            {
                MessageBox.Show(
                    "Selecciona un bloque primero.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(row["Id"]);
            byte[] rowVersion = row["RowVersion"] != DBNull.Value
                ? (byte[])row["RowVersion"]
                : null;

            string identificador = Convert.ToString(row["Identificador"]) ?? string.Empty;
            string mensaje = $"¿Deseas desactivar el bloque '{identificador}'?";

            using (var frm = new BloqueConfirmarDesactivacion(mensaje))
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

                        bool valido = nAuth.ValidarPassword(usuarioId, password);

                        if (!valido)
                            throw new InvalidOperationException("Contraseña incorrecta.");

                        _nBloque.Desactivar(id, rowVersion, usuarioLogin);
                        CargarBloques();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "No se pudo desactivar el bloque: " + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            chkSoloActivos.Checked = true;
            CargarBloques();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
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

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }
    }
}