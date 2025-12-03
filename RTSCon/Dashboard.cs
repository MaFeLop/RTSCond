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

namespace RTSCon
{
    public partial class Dashboard : KryptonForm
    {
        public Dashboard()
        {
            InitializeComponent();
            this.Shown += Dashboard_Shown;
            this.FormClosed += (s, e) => SessionManager.Stop();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var km = new KryptonManager();
            km.GlobalPaletteMode = PaletteMode.Office2010BlueLightMode; // o el que prefieras
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
            // 1) Limpiar el contexto de usuario
            UserContext.Clear();

            // 2) Abrir de nuevo el formulario de Login
            var login = new Login();
            login.Show();

            // 3) Cerrar este dashboard
            this.Close();
        }
    }
}
