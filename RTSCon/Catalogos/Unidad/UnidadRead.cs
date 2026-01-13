// RTSCon.Catalogos\UnidadRead.cs
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

        // ====== NUEVO: modo selección por checkbox ======
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

            dgvUnidades.AutoGenerateColumns = false;

            // NUEVO: columna selección + eventos para el checkbox
            EnsureSelectionColumn();
            dgvUnidades.CurrentCellDirtyStateChanged += dgvUnidades_CurrentCellDirtyStateChanged;
            dgvUnidades.CellContentClick += dgvUnidades_CellContentClick;
        }

        private void UnidadRead_Load(object sender, EventArgs e)
        {
            chkSoloActivos.Checked = true;
            CargarBloque();
            CargarUnidades();
            SalirModoSeleccion(); // por si acaso
        }

        private void CargarBloque()
        {
            try
            {
                if (_bloqueId <= 0) return;

                var rowBloque = _nBloque.PorId(_bloqueId);
                if (rowBloque != null)
                    Text = $"Unidades del bloque {rowBloque["Identificador"]}";
            }
            catch
            {
                // silencioso
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

                // Si estás en modo selección, asegúrate que la columna siga visible
                SetSelectionColumnVisible(_modo != ModoAccion.Ninguno);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar unidades: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataRow FilaSeleccionada()
        {
            if (dgvUnidades.CurrentRow == null) return null;
            var view = dgvUnidades.CurrentRow.DataBoundItem as DataRowView;
            return view?.Row;
        }

        // ====== NUEVO: selección por checkbox ======

        private void EnsureSelectionColumn()
        {
            if (dgvUnidades.Columns.Contains(COL_SEL)) return;

            var col = new DataGridViewCheckBoxColumn
            {
                Name = COL_SEL,
                HeaderText = "",
                Width = 28,
                ReadOnly = false,
                Visible = false
            };

            // que sea la primera columna
            dgvUnidades.Columns.Insert(0, col);
        }

        private void SetSelectionColumnVisible(bool visible)
        {
            if (!dgvUnidades.Columns.Contains(COL_SEL)) return;
            dgvUnidades.Columns[COL_SEL].Visible = visible;
        }

        // Para que el click en checkbox se registre al instante
        private void dgvUnidades_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvUnidades.IsCurrentCellDirty)
                dgvUnidades.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // Solo 1 check a la vez
        private void dgvUnidades_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_modo == ModoAccion.Ninguno) return;
            if (e.RowIndex < 0) return;
            if (dgvUnidades.Columns[e.ColumnIndex].Name != COL_SEL) return;

            // Si marcó uno, desmarca los demás
            var marcado = Convert.ToBoolean(dgvUnidades.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
            if (marcado)
            {
                for (int i = 0; i < dgvUnidades.Rows.Count; i++)
                {
                    if (i == e.RowIndex) continue;
                    dgvUnidades.Rows[i].Cells[COL_SEL].Value = false;
                }
            }
        }

        private DataRow GetRowMarcado()
        {
            if (_modo == ModoAccion.Ninguno) return null;

            foreach (DataGridViewRow gridRow in dgvUnidades.Rows)
            {
                if (gridRow.IsNewRow) continue;

                bool marcado = Convert.ToBoolean(gridRow.Cells[COL_SEL].Value ?? false);
                if (!marcado) continue;

                var view = gridRow.DataBoundItem as DataRowView;
                return view?.Row;
            }

            return null;
        }

        private void EntrarModoSeleccion(ModoAccion modo)
        {
            _modo = modo;

            // Mostrar columna checkbox
            SetSelectionColumnVisible(true);

            // Limpiar checks previos
            foreach (DataGridViewRow r in dgvUnidades.Rows)
            {
                if (r.IsNewRow) continue;
                r.Cells[COL_SEL].Value = false;
            }

            // UI: “armar” botón según modo
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

            // Volver funciona como “Cancelar selección”
            btnVolver.Text = "Cancelar";
        }

        private void SalirModoSeleccion()
        {
            _modo = ModoAccion.Ninguno;

            // Ocultar columna checkbox
            SetSelectionColumnVisible(false);

            // Reset de botones
            btnUpdate.Text = "Update";
            btnDesactivar.Text = "Desactivar";
            btnCrear.Enabled = true;
            btnUpdate.Enabled = true;
            btnDesactivar.Enabled = true;

            btnVolver.Text = "Volver";
        }

        // ====== BOTONES ======

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Si estamos en modo selección, ignorar
            if (_modo != ModoAccion.Ninguno) return;

            // Si no hay bloque aún, lo pedimos con BuscarBloque
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
            // 1er click: entra a modo selección
            if (_modo == ModoAccion.Ninguno)
            {
                EntrarModoSeleccion(ModoAccion.Editar);
                return;
            }

            // Si está en otro modo, no hagas nada
            if (_modo != ModoAccion.Editar) return;

            // 2do click: confirmar selección
            var row = GetRowMarcado();
            if (row == null)
            {
                MessageBox.Show("Marca una unidad (checkbox) para actualizar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];

            // Salimos del modo selección antes de abrir modal (más limpio)
            SalirModoSeleccion();

            using (var frm = new UpdateUnidad(id))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            // 1er click: entra a modo selección
            if (_modo == ModoAccion.Ninguno)
            {
                EntrarModoSeleccion(ModoAccion.Desactivar);
                return;
            }

            // Si está en otro modo, no hagas nada
            if (_modo != ModoAccion.Desactivar) return;

            // 2do click: confirmar selección
            var row = GetRowMarcado();
            if (row == null)
            {
                MessageBox.Show("Marca una unidad (checkbox) para desactivar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];
            byte[] rowVersion = (byte[])row["RowVersion"];
            string numero = row["Numero"].ToString();

            // Salimos del modo selección antes del confirm modal
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

                        // Validación (si tu ValidarPassword devuelve bool, valida el retorno)
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
            // Si estás seleccionando, esto actúa como cancelar selección
            if (_modo != ModoAccion.Ninguno)
            {
                SalirModoSeleccion();
                return;
            }

            Close();
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
