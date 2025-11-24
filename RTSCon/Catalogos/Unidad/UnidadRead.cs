using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class UnidadRead : Form
    {
        private readonly int _bloqueId;
        private readonly NUnidad _nUnidad;
        private readonly NBloque _nBloque;

        public UnidadRead(int bloqueId)
        {
            InitializeComponent();

            _bloqueId = bloqueId;

            var dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            dgvUnidades.AutoGenerateColumns = false;
        }

        private void UnidadRead_Load(object sender, EventArgs e)
        {
            chkSoloActivos.Checked = true;
            CargarBloque();
            CargarUnidades();
        }

        private void CargarBloque()
        {
            try
            {
                var rowBloque = _nBloque.PorId(_bloqueId);
                if (rowBloque != null)
                {
                    // label opcional para contexto, p.ej. "Bloque A (Condominio X)"
                    Text = $"Unidades del bloque {rowBloque["Identificador"]}";
                }
            }
            catch
            {
                // si falla, no rompemos el listado
            }
        }

        private void CargarUnidades()
        {
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                DataTable dt = _nUnidad.Buscar(_bloqueId, texto, soloActivos, 50);
                dgvUnidades.DataSource = dt;
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

        private void btnBuscar_Click(object sender, EventArgs e) => CargarUnidades();

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CargarUnidades();
            }
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e) => CargarUnidades();

        private DataRow FilaSeleccionada()
        {
            if (dgvUnidades.CurrentRow == null) return null;
            var view = dgvUnidades.CurrentRow.DataBoundItem as DataRowView;
            return view?.Row;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using (var frm = new CrearUnidad(_bloqueId))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var row = FilaSeleccionada();
            if (row == null)
            {
                MessageBox.Show("Selecciona una unidad primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];

            using (var frm = new UpdateUnidad(id))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    CargarUnidades();
            }
        }

        private void dgvUnidades_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnEditar_Click(sender, EventArgs.Empty);
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            var row = FilaSeleccionada();
            if (row == null)
            {
                MessageBox.Show("Selecciona una unidad primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];
            byte[] rowVersion = (byte[])row["RowVersion"];
            string numero = row["Numero"].ToString();

            string mensaje = $"¿Deseas desactivar la unidad '{numero}'?";

            using (var frm = new UnidadConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        int usuarioId = UserContext.UsuarioAuthId;
                        string usuario = UserContext.Usuario;
                        string password = frm.Password;

                        var dAuth = new DAuth(Conexion.CadenaConexion);
                        var nAuth = new NAuth(dAuth);
                        nAuth.ValidarPassword(usuarioId, password);

                        _nUnidad.Desactivar(id, rowVersion, usuario);

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

        private void btnCerrar_Click(object sender, EventArgs e) => Close();
    }
}
