using System;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class UnidadConfirmarDesactivacion : Form
    {
        public string Password => txtPassword.Text.Trim();

        public UnidadConfirmarDesactivacion(string mensaje)
        {
            InitializeComponent();
            lblMensaje.Text = mensaje;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Debes ingresar tu contraseña.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
