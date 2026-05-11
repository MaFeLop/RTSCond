using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class Dashboard : KryptonForm
    {
        private readonly NDashboard _dashboardNegocio;

        public Dashboard()
        {
            InitializeComponent();

            string cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _dashboardNegocio = new NDashboard(new DDashboard(cn));

            this.Shown -= Dashboard_Shown;
            this.Shown += Dashboard_Shown;

            this.FormClosed -= Dashboard_FormClosed;
            this.FormClosed += Dashboard_FormClosed;

            if (btnDashboard != null)
            {
                btnDashboard.Click -= btnDashboard_Click;
                btnDashboard.Click += btnDashboard_Click;
            }
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

            CargarDashboard();
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { SessionManager.Stop(); } catch { }
        }

        private void CargarDashboard()
        {
            PrepararVistaDashboard();

            try
            {
                DashboardResumen resumen = _dashboardNegocio.ObtenerResumen();
                MostrarResumen(resumen);
            }
            catch (Exception ex)
            {
                MostrarResumenEnError();

                KryptonMessageBox.Show(
                    this,
                    "No se pudo cargar el resumen del Dashboard.\n\nDetalle: " + ex.Message,
                    "Dashboard",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning
                );
            }
        }

        private void PrepararVistaDashboard()
        {
            if (DashboardName != null)
                DashboardName.Values.Text = "Dashboard";

            if (kryptonLabel1 != null)
            {
                kryptonLabel1.Visible = true;
                kryptonLabel1.Values.Text = ObtenerTextoSesion();
            }

            MostrarCard(
                CondominiosBorde,
                label1,
                label2,
                lblCondnum,
                "Condominios Activos:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge1,
                label5,
                label4,
                lblFacturasEmdedidas,
                "Bloques Activos:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge2,
                label7,
                label3,
                lblFacturado,
                "Unidades Activas:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge3,
                label10,
                label8,
                lblPagosRecibidos,
                "Propiedades Activas:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge4,
                label11,
                label6,
                lblDeudaVencida,
                "Usuarios Activos:",
                "0"
            );
        }

        private string ObtenerTextoSesion()
        {
            string usuario = !string.IsNullOrWhiteSpace(UserContext.Usuario)
                ? UserContext.Usuario
                : SessionHelper.Usuario;

            string rol = !string.IsNullOrWhiteSpace(UserContext.Rol)
                ? UserContext.Rol
                : SessionHelper.Rol;

            if (string.IsNullOrWhiteSpace(usuario))
                usuario = "N/A";

            if (string.IsNullOrWhiteSpace(rol))
                rol = "N/A";

            return "Usuario: " + usuario +
                   " | Rol: " + rol +
                   " | Fecha: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
        }

        private void MostrarResumen(DashboardResumen resumen)
        {
            if (resumen == null)
                resumen = new DashboardResumen();

            lblCondnum.Text = resumen.CondominiosActivos.ToString();
            lblFacturasEmdedidas.Text = resumen.BloquesActivos.ToString();
            lblFacturado.Text = resumen.UnidadesActivas.ToString();
            lblPagosRecibidos.Text = resumen.PropiedadesActivas.ToString();
            lblDeudaVencida.Text = resumen.UsuariosActivos.ToString();

            if (kryptonLabel1 != null)
                kryptonLabel1.Values.Text = ObtenerTextoSesion();
        }

        private void MostrarResumenEnError()
        {
            lblCondnum.Text = "N/A";
            lblFacturasEmdedidas.Text = "N/A";
            lblFacturado.Text = "N/A";
            lblPagosRecibidos.Text = "N/A";
            lblDeudaVencida.Text = "N/A";
        }

        private void MostrarCard(
            KryptonBorderEdge borde,
            Label titulo,
            Label icono,
            Label valor,
            string textoTitulo,
            string textoValor)
        {
            if (borde != null)
                borde.Visible = true;

            if (titulo != null)
            {
                titulo.Visible = true;
                titulo.Text = textoTitulo;
                titulo.TextAlign = ContentAlignment.MiddleCenter;
            }

            if (icono != null)
                icono.Visible = true;

            if (valor != null)
            {
                valor.Visible = true;
                valor.Text = textoValor;
                valor.TextAlign = ContentAlignment.MiddleCenter;
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            CargarDashboard();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionHelper.LogoutGlobal();
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            using (var frm = new CatalogoCRUD())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }

            CargarDashboard();
        }

        private void btnCrearPropietario_Click(object sender, EventArgs e)
        {
            using (var frm = new CrearUsuario())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }

            CargarDashboard();
        }
    }
}