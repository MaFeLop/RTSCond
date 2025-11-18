using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RTSCon.Entidad;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class BloqueRead : Form
    {
        private readonly int _condominioId;
        private readonly NBloque _servicioBloque; // TODO: ajusta a tu clase real

        public BloqueRead(int condominioId)
        {
            InitializeComponent();

            _condominioId = condominioId;
            _servicioBloque = new NBloque(); // TODO: o inyecta como lo haces en Condominio

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

                // TODO: usa la firma real de NBloque
                // Ejemplo si tienes algo como Buscar(condominioId, texto, soloActivos):
                var lista = _servicioBloque.Buscar(_condominioId, texto, soloActivos);

                dgvBloques.DataSource = lista.ToList();
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

        private EBloque ObtenerBloqueSeleccionado()
        {
            if (dgvBloques.CurrentRow == null)
                return null;

            // Igual que en CondominioRead: usamos DataBoundItem
            return dgvBloques.CurrentRow.DataBoundItem as EBloque;
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
            var bloque = ObtenerBloqueSeleccionado();
            if (bloque == null)
            {
                MessageBox.Show("Selecciona un bloque primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new UpdateBloque(bloque.Id))
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
            var bloque = ObtenerBloqueSeleccionado();
            if (bloque == null)
            {
                MessageBox.Show("Selecciona un bloque primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 🔐 Aquí reutilizas la MISMA pantalla de ConfirmarDesactivacion
            // que ya usas para Condominio/Propiedad.
            // Copia el MISMO código que tengas en CondominioRead, y solo cambia
            // la lógica interna para llamar a NBloque.Desactivar.

            string mensaje = $"¿Deseas desactivar el bloque '{bloque.Identificador}'?";

            using (var frm = new ConfirmarDesactivacion(mensaje))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string usuario = SesionActual.Usuario; // TODO: usa tu contexto real
                        string password = frm.Password;        // TODO: propiedad que ya tienes

                        // Igual que con Condominio:
                        // 1. Validar password (NAuth / DAuth)
                        // 2. Llamar a NBloque.Desactivar(id, rowVersion, usuario)

                        // Ejemplo:
                        // NAuth.ValidarPassword(usuario, password);
                        // _servicioBloque.Desactivar(bloque.Id, bloque.RowVersion, usuario);

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

        private void BloqueRead_Load_1(object sender, EventArgs e)
        {

        }
    }
}
