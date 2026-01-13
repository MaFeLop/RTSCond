// ==============================
// RTSCon\Login.cs
// ==============================
using System;
using System.Configuration;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace RTSCon
{
    public partial class Login : KryptonForm
    {
        private readonly RTSCon.Negocios.NAuth _auth;
        private int _usuarioAuthId;

        public Login()
        {
            InitializeComponent();

            var cs = ConfigurationManager.ConnectionStrings["RTSCond"]?.ConnectionString
                     ?? throw new InvalidOperationException(
                         "Falta connectionStrings['RTSCond'] en App.config.");

            _auth = new RTSCon.Negocios.NAuth(new RTSCon.Datos.DAuth(cs));
        }

        private void kryptonLabel2_Click(object sender, EventArgs e) { }
        private void kryptonLabel3_Click(object sender, EventArgs e) { }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = txtCorreo.Text.Trim();
                var password = txtContrasena.Text;

                var mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";
                var minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
                var debug = string.Equals(ConfigurationManager.AppSettings["CodigoDebug"], "true",
                                          StringComparison.OrdinalIgnoreCase);

                // 1) Validar usuario + contraseña (sin 2FA)
                _usuarioAuthId = _auth.Login_Password(usuario, password, mailProfile, minutosCodigo, debug);

                // 2) Marcar la sesión + UserContext
                _auth.Login_CodigoYSesion(_usuarioAuthId);

                // 3) Reset flag de cierre (MUY IMPORTANTE)
                SessionHelper.EndLogout();

                // 4) Abrir dashboard según rol
                Hide();

                Form next;
                var rol = RTSCon.Negocios.UserContext.Rol;

                if (string.Equals(rol, "SA", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    next = new Dashboard();
                }
                else
                {
                    next = new Dashboard(); // futuro: DashboardInquilino
                }

                next.Show();
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
            using (var f = new ContrasenaOlvidada(_auth))
                f.ShowDialog(this);
        }
    }
}
