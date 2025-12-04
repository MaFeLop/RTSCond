using Krypton.Toolkit;
using RTSCon.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTSCon;

namespace RTSCon
{
    public partial class Dashboard : KryptonForm
    {
        private SessionTimeoutBehavior _sessionTimeout;

        private readonly Timer _sessionTimer = new Timer();
        public Dashboard()
        {
            InitializeComponent();
            this.Shown += Dashboard_Shown;
            this.FormClosed += (s, e) => SessionManager.Stop();

            _sessionTimeout = new SessionTimeoutBehavior(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var km = new KryptonManager();
            km.GlobalPaletteMode = PaletteMode.Office2010BlueLightMode; // o el que prefieras
                                                                        // Intervalo de chequeo: cada 30 segundos
            _sessionTimer.Interval = 30_000;
            _sessionTimer.Tick += SessionTimer_Tick;
            _sessionTimer.Start();

            // Cualquier actividad resetea el contador
            this.MouseMove += ActivityDetected;
            this.KeyDown += ActivityDetected;
        }

        private void Dashboard_Shown(object sender, EventArgs e)
        {
            int idle = int.TryParse(ConfigurationManager.AppSettings["SessionIdleMinutes"], out var i) ? i : 30;
            int prompt = int.TryParse(ConfigurationManager.AppSettings["SessionPromptMinutes"], out var p) ? p : 25;

            SessionManager.Start(this, idle, prompt, onTimeout: () =>
            {
                // Limpiar contexto y volver al Login
                UserContext.Clear();
                try
                {
                    KryptonMessageBox.Show(this,
                        "Sesión finalizada por inactividad.",
                        "Sesión", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);
                }
                catch { /* si owner ya no está visible */ }

                this.BeginInvoke((Action)(() =>
                {
                    try
                    {
                        var login = new Login();
                        login.Show();
                        this.Close();
                    }
                    catch { Application.Exit(); }
                }));
            });
        }

        private void kryptonLabel1_Click(object sender, EventArgs e)
        {

        }

        private void kryptonLabel2_Click(object sender, EventArgs e)
        {

        }

        private void kryptonLabel2_Click_1(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionHelper.LogoutFrom(this);
        }

        private void ActivityDetected(object sender, EventArgs e)
        {
            RTSCon.Negocios.UserContext.Touch();
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            // Límite de inactividad (en minutos) – configurable
            int timeoutMinutes = int.TryParse(
                System.Configuration.ConfigurationManager.AppSettings["SessionTimeoutMinutes"],
                out var m) ? m : 15;   // default 15 min

            if (RTSCon.Negocios.UserContext.UsuarioAuthId == 0)
                return; // nadie logueado

            var inactivo = DateTime.UtcNow - RTSCon.Negocios.UserContext.UltimaActividadUtc;
            if (inactivo > TimeSpan.FromMinutes(timeoutMinutes))
            {
                _sessionTimer.Stop();
                this.MouseMove -= ActivityDetected;
                this.KeyDown -= ActivityDetected;

                Krypton.Toolkit.KryptonMessageBox.Show(
                    this,
                    "Su sesión ha expirado por inactividad.",
                    "Sesión expirada",
                    Krypton.Toolkit.KryptonMessageBoxButtons.OK,
                    Krypton.Toolkit.KryptonMessageBoxIcon.Information);

                SessionHelper.LogoutFrom(this);
            }
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            var frm = new CatalogoCRUD();
            frm.Show();
            this.Hide();   // escondemos el Dashboard mientras estás en catálogos
        }

    }
}
