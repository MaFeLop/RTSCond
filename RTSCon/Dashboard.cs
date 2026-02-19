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
            this.FormClosed += Dashboard_FormClosed;
        }

        private void Dashboard_Shown(object sender, EventArgs e)
        {
            int idle = int.TryParse(
                ConfigurationManager.AppSettings["SessionIdleMinutes"],
                out var i) ? i : 15;

            int prompt = int.TryParse(
                ConfigurationManager.AppSettings["SessionPromptMinutes"],
                out var p) ? p : 13;

            SessionManager.Start(this, idle, prompt);
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { SessionManager.Stop(); } catch { }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionHelper.LogoutGlobal();
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            var frm = new CatalogoCRUD();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
        }


        private void btnCrearPropietario_Click(object sender, EventArgs e)
        {
            var frm = new CrearUsuario();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
        }
    }
}
