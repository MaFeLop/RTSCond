using System;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class BloqueConfirmarDesactivacion : Form
    {
        public BloqueConfirmarDesactivacion(string mensaje)
        {
            InitializeComponent();
            lblDesactivacion.Text = mensaje;
        }

        // Exponemos la contraseña para que BloqueRead la lea
        public string Password => txtPassword.Text;

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Debes introducir tu contraseña.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
