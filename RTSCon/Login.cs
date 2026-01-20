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

            this.FormClosed += Login_FormClosed;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = txtCorreo.Text.Trim();
                string password = txtContrasena.Text.Trim();

                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                    throw new InvalidOperationException("Ingrese usuario y contraseña.");

                string mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "";
                int minutosCodigo = int.TryParse(
                    ConfigurationManager.AppSettings["CodigoExpiraMin"],
                    out var m) ? m : 10;

                int usuarioAuthId = _auth.Login_Password(
                    usuario,
                    password,
                    mailProfile,
                    minutosCodigo,
                    debug: false
                );

                _auth.Login_CodigoYSesion(usuarioAuthId);

                // ===== FIX ROL =====
                string rol;

                if (usuario.Equals("sa", StringComparison.OrdinalIgnoreCase))
                {
                    rol = "SA";
                }
                else
                {
                    rol = NormalizarRol(UserContext.Rol);
                }

                if (string.IsNullOrWhiteSpace(rol))
                    throw new InvalidOperationException("No se pudo determinar el rol del usuario.");

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
                    KryptonMessageBoxIcon.Error);
            }
        }

        private string NormalizarRol(string rolBd)
        {
            if (string.IsNullOrWhiteSpace(rolBd)) return "";

            rolBd = rolBd.Trim().ToUpperInvariant();

            if (rolBd.Contains("SA") || rolBd.Contains("SUPER"))
                return "SA";

            if (rolBd.Contains("PROPIET"))
                return "Propietario";

            if (rolBd.Contains("SECRE"))
                return "Secretario";

            if (rolBd.Contains("INQUI"))
                return "Inquilino";

            return "";
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            ResetFormulario();
            this.Show();
            this.Activate();
        }

        private void ResetFormulario()
        {
            txtCorreo.Text = string.Empty;
            txtContrasena.Text = string.Empty;
            txtCorreo.Focus();
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void lnkForget_LinkClicked(object sender, EventArgs e)
        {
            try
            {
                var frm = new ContrasenaOlvidada(_auth);
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo abrir la recuperación de contraseña: " + ex.Message,
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

    }
}
