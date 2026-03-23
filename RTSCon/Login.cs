using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class Login : KryptonForm
    {
        private readonly NAuth _auth;

        public Login()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _auth = new NAuth(new DAuth(cn));

            this.FormClosed += (s, e) => Application.Exit();
        }

        private void LimpiarCredenciales(bool limpiarUsuario = true)
        {
            if (limpiarUsuario && txtCorreo != null)
                txtCorreo.Clear();

            if (txtContrasena != null)
            {
                txtContrasena.Clear();
                txtContrasena.PasswordChar = '●';
                txtContrasena.UseSystemPasswordChar = true;
            }

            if (limpiarUsuario && txtCorreo != null)
                txtCorreo.Focus();
            else if (txtContrasena != null)
                txtContrasena.Focus();
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtCorreo?.Text?.Trim() ?? string.Empty;
                string clave = txtContrasena?.Text ?? string.Empty;

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new InvalidOperationException("Ingrese su nombre de usuario.");

                if (string.IsNullOrWhiteSpace(clave))
                    throw new InvalidOperationException("Ingrese su contraseña.");

                int id = _auth.Login(usuario, clave);

                SessionHelper.Start(usuario, UserContext.Rol, id);

                LimpiarCredenciales();

                this.Hide();

                using (var dashboard = new Dashboard())
                {
                    dashboard.ShowDialog();
                }

                this.Show();
                LimpiarCredenciales();
            }
            catch (Exception ex)
            {
                LimpiarCredenciales(true);
                MessageBox.Show(ex.Message);
            }
        }

        private void lnkForget_LinkClicked(object sender, EventArgs e)
        {
            try
            {
                var frm = new ContrasenaOlvidada(_auth)
                {
                    StartPosition = FormStartPosition.CenterParent
                };

                frm.ShowDialog(this);

                if (txtContrasena != null)
                    txtContrasena.Clear();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo abrir la recuperación: " + ex.Message,
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error
                );
            }
        }
    }
}