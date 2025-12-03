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

        private void kryptonLabel2_Click(object sender, EventArgs e)
        {
        }

        private void kryptonLabel3_Click(object sender, EventArgs e)
        {
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Este textbox debe contener el "usuario" tal como está en UsuarioAuth (Usuario o Correo, según tu SP).
                var usuario = txtCorreo.Text.Trim();
                var password = txtContrasena.Text;

                // Se mantienen estos parámetros solo para compatibilidad de firma con Login_Password
                var mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";
                var minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
                var debug = string.Equals(ConfigurationManager.AppSettings["CodigoDebug"], "true",
                                          StringComparison.OrdinalIgnoreCase);

                // 1) Validar usuario + contraseña (sin 2FA)
                _usuarioAuthId = _auth.Login_Password(usuario, password, mailProfile, minutosCodigo, debug);

                // 2) Marcar la sesión directamente
                _auth.Login_CodigoYSesion(_usuarioAuthId);

                // 3) Abrir dashboard según rol
                Hide();

                Form next;
                var rol = RTSCon.Negocios.UserContext.Rol; // SA | Admin | Inquilino

                if (string.Equals(rol, "SA", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    next = new Dashboard();              // dashboard administrador
                }
                else
                {
                    next = new Dashboard();              // más adelante: DashboardInquilino
                    // next = new DashboardInquilino(RTSCon.Negocios.UserContext.UsuarioAuthId);
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
