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

                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                    throw new InvalidOperationException("Ingrese usuario y contraseña.");

                string mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";
                int minutosCodigo = int.TryParse(
                    ConfigurationManager.AppSettings["CodigoExpiraMin"],
                    out var m) ? m : 10;

                // 1️⃣ Login por password (devuelve ID)
                int usuarioAuthId = _auth.Login_Password(
                    usuario,
                    password,
                    mailProfile,
                    minutosCodigo,
                    debug: false
                );

                // 2️⃣ Abrir verificación por código
                using (var frmCodigo = new LoginCodigo(_auth, usuarioAuthId))
                {
                    frmCodigo.StartPosition = FormStartPosition.CenterParent;

                    if (frmCodigo.ShowDialog(this) != DialogResult.OK)
                        return; // código incorrecto o cancelado
                }

                // 3️⃣ Sesión válida → Dashboard
                string rol = NormalizarRol(UserContext.Rol);

                SessionHelper.Start(
                    usuario: usuario,
                    rol: rol,
                    usuarioId: usuarioAuthId
                );

                var dash = new Dashboard
                {
                    StartPosition = FormStartPosition.CenterScreen
                };

                dash.FormClosed += Dashboard_FormClosed;
                dash.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    ex.Message,
                    "Login",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error
                );
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
