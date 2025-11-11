using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
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
                // Asegúrate de que este textbox contenga el NOMBRE DE USUARIO real.
                var usuario = txtCorreo.Text.Trim();   // si tu control es txtUsuario, cámbialo aquí
                var password = txtContrasena.Text;

                var mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";
                var minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
                var debug = string.Equals(ConfigurationManager.AppSettings["CodigoDebug"], "true", StringComparison.OrdinalIgnoreCase);

                // Paso 1: valida y ENVÍA código → y guarda el ID en el campo de clase
                _usuarioAuthId = _auth.Login_Password(usuario, password, mailProfile, minutosCodigo, debug);

                // Mensaje solicitado
                KryptonMessageBox.Show(
                    this,
                    "Su usuario es correcto, se le envió un mensaje a su correo vinculado.\n" +
                    "Valide ahora su correo para culminar el inicio de sesión.",
                    "Verificación requerida",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                // Paso 2: formulario de código 2FA
                using (var f = new LoginCodigo(_auth, _usuarioAuthId))
                {
                    if (f.ShowDialog(this) == DialogResult.OK)
                    {
                        _auth.Login_CodigoYSesion(_usuarioAuthId);
                        Hide();

                        Form next;
                        var rol = RTSCon.Negocios.UserContext.Rol; // SA | Admin | Inquilino

                        if (string.Equals(rol, "SA", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            next = new Dashboard();              // tu dashboard de administrador
                        }
                        else
                        {
                            next = new Dashboard();              // si aún no tienes DashboardInquilino, deja el mismo
                                                                 // next = new DashboardInquilino(RTSCon.Negocios.UserContext.UsuarioAuthId);
                        }
                        next.Show();
                    }
                }
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
            using (var f = new ContrasenaOlvidada(_auth)) // le pasamos el mismo NAuth
                f.ShowDialog(this);
        }
    }
}
