// ===== RTSCon/Login.cs =====
using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class Login : KryptonForm
    {
        private readonly NAuth _auth;

        public Login()
        {
            InitializeComponent();

            _auth = new NAuth(new DAuth(Conexion.CadenaConexion));

            // Si el usuario cierra Login, salir completo
            this.FormClosed += (_, __) => Application.ExitThread();

            // Hook robusto por nombres estándar
            HookButtons();
            HookEnterToLogin();
        }

        private void HookButtons()
        {
            var btnIngresar = FindCtrl<Control>("btnIngresar", "btnLogin", "btnEntrar");
            if (btnIngresar != null) btnIngresar.Click += btnIngresar_Click;

            var btnSalir = FindCtrl<Control>("btnSalir", "btnCerrar", "btnCancelar");
            if (btnSalir != null) btnSalir.Click += (_, __) => Close();
        }

        private void HookEnterToLogin()
        {
            var txtUsuario = FindCtrl<TextBoxBase>("txtUsuario", "txtLogin", "txtCorreo");
            var txtClave = FindCtrl<TextBoxBase>("txtPassword", "txtClave", "txtContrasena", "txtContraseña");

            if (txtUsuario != null) txtUsuario.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnIngresar_Click(this, EventArgs.Empty); }
            };

            if (txtClave != null) txtClave.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnIngresar_Click(this, EventArgs.Empty); }
            };
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = (FindCtrl<TextBoxBase>("txtUsuario", "txtLogin", "txtCorreo")?.Text ?? "").Trim();
                string clave = (FindCtrl<TextBoxBase>("txtPassword", "txtClave", "txtContrasena", "txtContraseña")?.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
                    throw new InvalidOperationException("Ingrese usuario/correo y contraseña.");

                // Ajustes por App.config (si existen)
                string mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCon";
                int minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["MinutosCodigo"], out var m) ? m : 10;

                // Login (Password + envío de código si tu SP lo hace)
                int usuarioId = _auth.Login_Password(usuario, clave, mailProfile, minutosCodigo, debug: false);

                // Si existe la pantalla de código (2FA) en tu solución, abrirla sin referenciarla fuerte (reflection)
                if (TryShowLoginCodigo(usuarioId))
                    return;

                // Si no hay LoginCodigo, pasar directo a Dashboard
                AbrirDashboardYSalir();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message, "Login",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }

        private bool TryShowLoginCodigo(int usuarioId)
        {
            // Busca: RTSCon.LoginCodigo o RTSCon.LoginCodigoForm o RTSCon.LoginConCodigo, etc.
            var asm = Assembly.GetExecutingAssembly();
            var t =
                asm.GetType("RTSCon.LoginCodigo") ??
                asm.GetType("RTSCon.LoginCodigoForm") ??
                asm.GetType("RTSCon.LoginConCodigo");

            if (t == null) return false;

            try
            {
                // Crea instancia y pásale el usuarioId por Tag (patrón que ya usas)
                using (var frm = Activator.CreateInstance(t) as Form)
                {
                    if (frm == null) return false;

                    frm.Tag = usuarioId;

                    this.Hide();
                    frm.StartPosition = FormStartPosition.CenterScreen;

                    var dr = frm.ShowDialog(this);

                    // Si el código fue OK, abrir Dashboard
                    if (dr == DialogResult.OK)
                    {
                        AbrirDashboardYSalir();
                        return true;
                    }

                    // Si canceló, regresar al Login visible
                    this.Show();
                    this.Activate();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void AbrirDashboardYSalir()
        {
            // Dashboard (admin/propietario) – si tienes otros dashboards por rol, puedes decidir aquí
            using (var dash = new Dashboard())
            {
                this.Hide();
                dash.StartPosition = FormStartPosition.CenterScreen;
                dash.ShowDialog(this);
            }

            // Al cerrar Dashboard, cerrar Login => Application.ExitThread (FormClosed)
            this.Close();
        }

        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (var n in names)
            {
                var c = this.Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null) return c;
            }
            return null;
        }
    }
}
