using Krypton.Toolkit;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class Dashboard : KryptonForm
    {
        private readonly Login _login;

        public Dashboard() : this(null) { }

        public Dashboard(Login login)
        {
            InitializeComponent();
            _login = login;

            this.Shown += Dashboard_Shown;
            this.FormClosed += (_, __) =>
            {
                try { SessionManager.Stop(); } catch { }
            };
        }

        private void Dashboard_Shown(object sender, EventArgs e)
        {
            int idle = int.TryParse(ConfigurationManager.AppSettings["SessionIdleMinutes"], out var i) ? i : 30;
            int prompt = int.TryParse(ConfigurationManager.AppSettings["SessionPromptMinutes"], out var p) ? p : 25;

            SessionManager.Start(this, idle, prompt);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // ✅ Logout debe volver al Login, NO cerrar la app.
            try
            {
                try { SessionManager.Stop(); } catch { }

                // Limpieza de sesión (si tu helper existe)
                try { SessionHelper.LogoutFrom(this); } catch { }

                if (_login != null)
                {
                    _login.VolverDesdeLogout();
                }

                this.Close(); // cierra Dashboard y deja Login vivo
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo cerrar sesión: " + ex.Message,
                    "Dashboard",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        // Tu botón de Catálogos (como lo tenías)
        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            var frm = new CatalogoCRUD();
            frm.FormClosed += (_, __) =>
            {
                try { this.Show(); this.Activate(); } catch { }
            };
            frm.Show();
            this.Hide();
        }

        // Crear Usuario / Propietario
        private void btnCrearPropietario_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new CrearUsuario())
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo abrir Crear Usuario: " + ex.Message,
                    "Dashboard",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }
    }
}
