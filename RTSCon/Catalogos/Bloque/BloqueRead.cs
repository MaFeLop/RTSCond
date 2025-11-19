using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;
// using RTSCon.Seguridad;  // donde tengas UserContext / NAuth

namespace RTSCon.Catalogos
{
    public partial class BloqueRead : Form
    {
        private readonly int _condominioId;
        private readonly NBloque _servicioBloque;

        public BloqueRead(int condominioId)
        {
            InitializeComponent();

            _condominioId = condominioId;

            // ⬇️ Usa EXACTAMENTE la misma forma de obtener la cadena que en DCondominio
            var dBloque = new DBloque(Conexion.CadenaConexion); // <-- cambia por tu helper real
            _servicioBloque = new NBloque(dBloque);

            dgvBloques.AutoGenerateColumns = false; // igual que en CondominioRead
        }

        private void BloqueRead_Load(object sender, EventArgs e)
        {
            chkSoloActivos.Checked = true;
            CargarBloques();
        }

        private void CargarBloques()
        {
            try
            {
                string texto = txtBuscar.Text.Trim();
                bool soloActivos = chkSoloActivos.Checked;

                // NBloque.Buscar devuelve DataTable, no lista
                DataTable dt = _servicioBloque.Buscar(_condominioId, texto, soloActivos, 50);

                dgvBloques.DataSource = dt;
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

        private void btnBuscar_Click(object sender, EventArgs e)
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

        // ⬇️ Ahora trabajamos con DataRow, no con EBloque
        private DataRow FilaSeleccionada()
        {
            if (dgvBloques.CurrentRow == null) return null;

            var rowView = dgvBloques.CurrentRow.DataBoundItem as DataRowView;
            return rowView?.Row;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using (var frm = new CrearBloque(_condominioId))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    CargarBloques();
                }
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var row = FilaSeleccionada();
            if (row == null)
            {
                MessageBox.Show("Selecciona un bloque primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];

            using (var frm = new UpdateBloque(id))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    CargarBloques();
                }
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
                MessageBox.Show("Selecciona un bloque primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];
            byte[] rowVersion = (byte[])row["RowVersion"];
            string identificador = row["Identificador"].ToString();

            string mensaje = $"¿Deseas desactivar el bloque '{identificador}'?";

            // ⚠️ Usa el nombre REAL de tu form de confirmación.
            // En el explorador se ve "BloqueConfirmarDesactivacion.cs"
            using (var frm = new BloqueConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string usuario = UserContext.Usuario;  // igual que en CondominioRead
                        string password = frm.Password;        // misma propiedad que ya usas

                        // 1) Validar password (igualito a Condominio)
                        // NAuth auth = new NAuth(...);
                        // auth.ValidarPassword(usuario, password);

                        // 2) Desactivar bloque
                        _servicioBloque.Desactivar(id, rowVersion, usuario);

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

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
