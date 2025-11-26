using System;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace RTSCon.Catalogos
{
    public partial class PropiedadConfirmarDesactivacion : KryptonForm
    {
        // Exponemos la contraseña que el usuario escribió
        public string Password => txtPassword.Text.Trim();

        public PropiedadConfirmarDesactivacion(string mensaje)
        {
            InitializeComponent();

            // Ajusta el nombre del label si en tu diseñador se llama distinto
            lblDesactivacion.Text = mensaje;

            txtPassword.UseSystemPasswordChar = true;
        }

        private void PropiedadConfirmarDesactivacion_Load(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Ingrese su contraseña.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
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

        // Enter en el textbox = click en Aceptar
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnConfirmar.PerformClick();
            }
        }
    }
}
