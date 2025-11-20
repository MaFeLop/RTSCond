using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class BloqueRead : Form
    {
        private readonly int _condominioId;
        private readonly NBloque _nBloque;

        public BloqueRead(int condominioId)
        {
            InitializeComponent();

            _condominioId = condominioId;

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            dgvBloques.AutoGenerateColumns = false;
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

                DataTable dt = _nBloque.Buscar(_condominioId, texto, soloActivos, 50);
                dgvBloques.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar bloques: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => CargarBloques();

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CargarBloques();
            }
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e) => CargarBloques();

        private DataRow FilaSeleccionada()
        {
            if (dgvBloques.CurrentRow == null) return null;
            var view = dgvBloques.CurrentRow.DataBoundItem as DataRowView;
            return view?.Row;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show("Selecciona un bloque primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];

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
                MessageBox.Show("Selecciona un bloque primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)row["Id"];
            byte[] rowVersion = (byte[])row["RowVersion"];
            string identificador = row["Identificador"].ToString();

            string mensaje = $"¿Deseas desactivar el bloque '{identificador}'?";

            using (var frm = new BloqueConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        
                        int usuarioId = UserContext.UsuarioAuthId;   // ID numérico
                        string usuario = UserContext.Usuario;        // nombre / correo
                        string password = frm.Password;

                        var dAuth = new DAuth(Conexion.CadenaConexion);
                        var nAuth = new NAuth(dAuth);
                        nAuth.ValidarPassword(usuarioId, password);  // aquí va el int

                        _nBloque.Desactivar(id, rowVersion, usuario);

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

        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private void txtBuscar_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CargarBloques();
            }
        }
    }
}
