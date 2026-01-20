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
                out var i) ? i : 30;

            int prompt = int.TryParse(
                ConfigurationManager.AppSettings["SessionPromptMinutes"],
                out var p) ? p : 25;

            SessionManager.Start(this, idle, prompt);
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { SessionManager.Stop(); } catch { }
            // NO cerrar la app aquí.
            // Login se mostrará automáticamente porque nunca se cerró.
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                SessionHelper.LogoutFrom(this);
            }
            catch
            {
                this.Close();
            }
        }


        // ====== CATÁLOGOS / CRUD ======
        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new CatalogoCRUD();
                frm.FormClosed += (_, __) =>
                {
                    try
                    {
                        this.Show();
                        this.Activate();
                    }
                    catch { }
                };

                frm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo abrir Catálogos: " + ex.Message,
                    "Dashboard",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        // ====== CREAR USUARIO / PROPIETARIO ======
        private void btnCrearPropietario_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new CrearUsuario();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
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
