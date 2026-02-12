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

        private string NormalizarRol(string rol)
        {
            if (string.IsNullOrWhiteSpace(rol))
                return string.Empty;

            return rol.Trim().ToUpperInvariant();
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Al cerrar el dashboard, cerrar también el login
            this.Close();
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtCorreo.Text.Trim();
                string password = txtContrasena.Text.Trim();

                int usuarioAuthId = _auth.Login(usuario, password);

                SessionHelper.Start(
                    usuario: usuario,
                    rol: UserContext.Rol,
                    usuarioId: usuarioAuthId
                );

                var dash = new Dashboard();
                dash.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message);
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

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword("sa1234");
            MessageBox.Show(hash);

        }
    }
}
