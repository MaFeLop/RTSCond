using Krypton.Toolkit;
using RTSCon.Negocios;
using System;
using System.Configuration;
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

        private void Dashboard_Shown(object sender, EventArgs e)
        {
            int idle = int.TryParse(ConfigurationManager.AppSettings["SessionIdleMinutes"], out var i) ? i : 30;
            int prompt = int.TryParse(ConfigurationManager.AppSettings["SessionPromptMinutes"], out var p) ? p : 25;

            // ✅ Sin onTimeout, porque SessionManager ya hace logout internamente.
            SessionManager.Start(this, idle, prompt);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionHelper.LogoutFrom(this);
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            var frm = new CatalogoCRUD();
            frm.Show();
            this.Hide();   // escondemos el Dashboard mientras estás en catálogos
        }

        // Si tienes handlers vacíos, puedes dejarlos o borrarlos
        private void kryptonLabel1_Click(object sender, EventArgs e) { }
        private void kryptonLabel2_Click(object sender, EventArgs e) { }
        private void kryptonLabel2_Click_1(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void btnDashboard_Click(object sender, EventArgs e) { }
    }
}
