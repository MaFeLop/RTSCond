using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Linq;
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

            // ✅ Si cierran Login, termina la app.
            this.FormClosed += (_, __) => Application.Exit();

            // (Opcional) Asegurar que el botón esté conectado aunque el Designer falle
            this.Shown += (_, __) => HookButtonsSafe();
        }

        private void HookButtonsSafe()
        {
            var btnIngresar = this.Controls.Find("btnIngresar", true).FirstOrDefault();
            if (btnIngresar != null) btnIngresar.Click -= btnIngresar_Click;
            if (btnIngresar != null) btnIngresar.Click += btnIngresar_Click;

            var btnSalir = this.Controls.Find("btnSalir", true).FirstOrDefault();
            if (btnSalir != null)
            {
                btnSalir.Click -= BtnSalir_Click;
                btnSalir.Click += BtnSalir_Click;
            }
        }

        private void BtnSalir_Click(object sender, EventArgs e) => this.Close();

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = (Controls.Find("txtUsuario", true).FirstOrDefault()?.Text ?? "").Trim();
                string password = (Controls.Find("txtClave", true).FirstOrDefault()?.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                    throw new InvalidOperationException("Ingrese usuario y contraseña.");

                string mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "";
                int minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoExpiraMin"], out var m) ? m : 10;

                // ✅ Login real (NAuth)
                int usuarioAuthId = _auth.Login_Password(usuario, password, mailProfile, minutosCodigo, debug: false);
                _auth.Login_CodigoYSesion(usuarioAuthId);

                // ✅ Abrir Dashboard y ocultar Login
                var dash = new Dashboard();
                dash.StartPosition = FormStartPosition.CenterScreen;

                // cuando Dashboard cierre (logout), Login vuelve
                dash.FormClosed += (_, __) => VolverDesdeLogout();

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

        public void VolverDesdeLogout()
        {
            try
            {
                this.Show();
                this.Activate();
            }
            catch { }
        }
    }
}
