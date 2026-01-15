// ===== RTSCon/Dashboard.cs (REEMPLAZAR COMPLETO) =====
using Krypton.Toolkit;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class Dashboard : KryptonForm
    {
        public Dashboard()
        {
            InitializeComponent();

            this.Shown += Dashboard_Shown;
            this.FormClosed += (s, e) =>
            {
                try { SessionManager.Stop(); } catch { }
                // OJO: no fuerces Application.ExitThread aquí si tu flujo ya hace logout/cierre.
                // Si ves que se queda “corriendo”, lo correcto es arreglar Program.cs (Application.Run)
                // o cerrar el form principal, no matar el hilo a la fuerza desde aquí.
            };

            // Hook adicional (no rompe los eventos que ya existan en el diseñador)
            this.Shown += (_, __) => HookButtonsSafe();
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

        // ======== TU BOTÓN DE CATÁLOGOS (como lo tenías) ========
        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            var frm = new CatalogoCRUD();
            frm.Show();
            this.Hide();   // escondemos el Dashboard mientras estás en catálogos
        }

        // ======== Handlers vacíos (los dejo igual) ========
        private void kryptonLabel1_Click(object sender, EventArgs e) { }
        private void kryptonLabel2_Click(object sender, EventArgs e) { }
        private void kryptonLabel2_Click_1(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void btnDashboard_Click(object sender, EventArgs e) { }

        // ======== TU BOTÓN CREAR PROPIETARIO (como lo tenías) ========
        private void btnCrearPropietario_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();

                using (var frm = new CrearUsuario())
                {
                    frm.ShowDialog(this);
                }

                this.Show();
                this.Activate();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo abrir Crear Propietario: " + ex.Message,
                    "Dashboard",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        // ==========================================================
        //          FIX: Hook FLEXIBLE (NO da CS7036)
        // ==========================================================
        private void HookButtonsSafe()
        {
            // Si ya tienes eventos asignados desde el diseñador, esto NO los rompe:
            // solo añade un handler extra si encuentra el control por Name.

            // Botón Catálogos (si el Name real es btnDashboard o btnCatalogoCRUD etc.)
            Hook(
                () => OpenFormNonModalHideThis(typeof(CatalogoCRUD)),
                "btnDashboard",
                "btnCatalogoCRUD",
                "btnCatalogos",
                "btnCRUD",
                "kryptonButton4" // por si el Name real es ese
            );

            // Crear usuario/propietario (si el Name real es btnCrearPropietario o parecido)
            Hook(
                () => OpenDialog(typeof(CrearUsuario)),
                "btnCrearPropietario",
                "btnCrearUsuario",
                "btnUsuarios",
                "btnUserCreate"
            );

            // Si luego quieres conectar más botones del Dashboard, los agregas aquí.
        }

        /// <summary>
        /// Hook flexible: recibe 1..N nombres y una acción.
        /// </summary>
        private void Hook(Action action, params string[] controlNames)
        {
            if (action == null || controlNames == null || controlNames.Length == 0) return;

            var ctrl = FindCtrl<Control>(controlNames);
            if (ctrl == null) return;

            ctrl.Click += (_, __) => action();
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

        private void OpenDialog(Type formType)
        {
            using (var frm = Activator.CreateInstance(formType) as Form)
            {
                if (frm == null) return;
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
        }

        private void OpenFormNonModalHideThis(Type formType)
        {
            var frm = Activator.CreateInstance(formType) as Form;
            if (frm == null) return;

            // Cuando cierre el hijo, mostramos Dashboard otra vez
            frm.FormClosed += (_, __) =>
            {
                try { this.Show(); this.Activate(); } catch { }
            };

            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
            this.Hide();
        }
    }
}
