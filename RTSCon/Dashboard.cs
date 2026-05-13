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
        #region 01 - Campos

        private readonly NDashboard _dashboardNegocio;

        #endregion

        #region 02 - Constructor

        public Dashboard()
        {
            InitializeComponent();

            _dashboardNegocio = CrearServicioDashboard();

            ConfigurarEventos();
            ConfigurarSidebar();
        }

        private NDashboard CrearServicioDashboard()
        {
            string cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            return new NDashboard(new DDashboard(cn));
        }

        #endregion

        #region 03 - Configuración inicial

        private void ConfigurarEventos()
        {
            this.Shown -= Dashboard_Shown;
            this.Shown += Dashboard_Shown;

            this.FormClosed -= Dashboard_FormClosed;
            this.FormClosed += Dashboard_FormClosed;

            if (btnDashboard != null)
            {
                btnDashboard.Click -= btnDashboard_Click;
                btnDashboard.Click += btnDashboard_Click;
            }

            if (kryptonButton4 != null)
            {
                kryptonButton4.Click -= kryptonButton4_Click;
                kryptonButton4.Click += kryptonButton4_Click;
            }

            if (btnCrearPropietario != null)
            {
                btnCrearPropietario.Click -= btnCrearPropietario_Click;
                btnCrearPropietario.Click += btnCrearPropietario_Click;
            }

            if (btnLogout != null)
            {
                btnLogout.Click -= btnLogout_Click;
                btnLogout.Click += btnLogout_Click;
            }
        }

        private void ConfigurarSidebar()
        {
            // Módulos no funcionales por ahora.
            if (label15 != null) label15.Visible = false;
            if (btnCierreDeMes != null) btnCierreDeMes.Visible = false;

            if (label16 != null) label16.Visible = false;
            if (btnFacturacion != null) btnFacturacion.Visible = false;

            if (label17 != null) label17.Visible = false;
            if (kryptonButton1 != null) kryptonButton1.Visible = false;

            if (label18 != null) label18.Visible = false;
            if (kryptonButton2 != null) kryptonButton2.Visible = false;

            if (label19 != null) label19.Visible = false;
            if (kryptonButton3 != null) kryptonButton3.Visible = false;
        }

        #endregion

        #region 04 - Eventos principales del formulario

        private void Dashboard_Shown(object sender, EventArgs e)
        {
            IniciarControlSesion();
            CargarDashboard();
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SessionManager.Stop();
            }
            catch
            {
            }
        }

        #endregion

        #region 05 - Control de sesión

        private void IniciarControlSesion()
        {
            int idle;
            int prompt;

            if (!int.TryParse(ConfigurationManager.AppSettings["SessionIdleMinutes"], out idle))
                idle = 15;

            if (!int.TryParse(ConfigurationManager.AppSettings["SessionPromptMinutes"], out prompt))
                prompt = 13;

            SessionManager.Start(this, idle, prompt);
        }

        private string ObtenerUsuarioActual()
        {
            string usuario = !string.IsNullOrWhiteSpace(UserContext.Usuario)
                ? UserContext.Usuario
                : SessionHelper.Usuario;

            return string.IsNullOrWhiteSpace(usuario) ? "N/A" : usuario;
        }

        private string ObtenerRolActual()
        {
            string rol = !string.IsNullOrWhiteSpace(UserContext.Rol)
                ? UserContext.Rol
                : SessionHelper.Rol;

            return string.IsNullOrWhiteSpace(rol) ? "N/A" : rol;
        }

        private string ObtenerTextoSesion()
        {
            return "Rol: " + ObtenerRolActual() +
                   " | Fecha: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
        }

        #endregion

        #region 06 - Carga general del Dashboard

        private void CargarDashboard()
        {
            PrepararVistaDashboard();
            CargarResumenGeneral();
            CargarUltimosEventos();
        }

        private void PrepararVistaDashboard()
        {
            PrepararEncabezado();
            PrepararCardsResumen();
            PrepararPanelUltimosEventos();
        }

        private void PrepararEncabezado()
        {
            if (DashboardName != null)
                DashboardName.Values.Text = "Dashboard";

            if (lblEstatusUsuario != null)
            {
                lblEstatusUsuario.Visible = true;
                lblEstatusUsuario.Values.Text = "Bienvenido: " + ObtenerUsuarioActual();
            }

            if (kryptonLabel1 != null)
            {
                kryptonLabel1.Visible = true;
                kryptonLabel1.Values.Text = ObtenerTextoSesion();
            }
        }

        #endregion

        #region 07 - Resumen general del sistema

        private void PrepararCardsResumen()
        {
            MostrarCard(
                CondominiosBorde,
                lblCondActivos,
                label2,
                lblCondNum,
                "Condominios Activos:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge1,
                lblBloquesActivos,
                label4,
                lblBloqNum,
                "Bloques Activos:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge2,
                lblUniActiv,
                label3,
                lblUniNum,
                "Unidades Activas:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge3,
                lblPropAct,
                label8,
                lblPropNum,
                "Propiedades Activas:",
                "0"
            );

            MostrarCard(
                kryptonBorderEdge4,
                lblUsuAct,
                label6,
                lblUsuNum,
                "Usuarios Activos:",
                "0"
            );
        }

        private void CargarResumenGeneral()
        {
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

        private void MostrarResumen(DashboardResumen resumen)
        {
            if (resumen == null)
                resumen = new DashboardResumen();

            lblCondNum.Text = resumen.CondominiosActivos.ToString();
            lblBloqNum.Text = resumen.BloquesActivos.ToString();
            lblUniNum.Text = resumen.UnidadesActivas.ToString();
            lblPropNum.Text = resumen.PropiedadesActivas.ToString();
            lblUsuNum.Text = resumen.UsuariosActivos.ToString();

            PrepararEncabezado();
        }

        private void MostrarResumenEnError()
        {
            lblCondNum.Text = "N/A";
            lblBloqNum.Text = "N/A";
            lblUniNum.Text = "N/A";
            lblPropNum.Text = "N/A";
            lblUsuNum.Text = "N/A";
        }

        #endregion

        #region 08 - Últimos eventos del sistema

        private void PrepararPanelUltimosEventos()
        {
            if (kryptonBorderEdge5 != null)
                kryptonBorderEdge5.Visible = true;

            if (label13 != null)
            {
                label13.Visible = true;
                label13.Text = "Últimos Eventos:";
            }

            if (label14 != null)
            {
                label14.Visible = true;
                label14.Text = "Actividad reciente del sistema";
            }

            MostrarLineaEvento(kryptonBorderEdge6, lblFacturacion, StatusFactura);
            MostrarLineaEvento(kryptonBorderEdge7, lblDunning, StatusDunning);
            MostrarLineaEvento(kryptonBorderEdge8, lblPagos, StatusPagos);
            MostrarLineaEvento(kryptonBorderEdge9, lblValidacion, StatusValidacion);
            MostrarLineaEvento(kryptonBorderEdge10, lblInicioMes, StatusInicioMes);

            if (kryptonBorderEdge11 != null)
                kryptonBorderEdge11.Visible = false;
        }

        private void CargarUltimosEventos()
        {
            try
            {
                var eventos = _dashboardNegocio.ObtenerUltimosEventos();

                PintarEvento(0, eventos.Count > 0 ? eventos[0] : null);
                PintarEvento(1, eventos.Count > 1 ? eventos[1] : null);
                PintarEvento(2, eventos.Count > 2 ? eventos[2] : null);
                PintarEvento(3, eventos.Count > 3 ? eventos[3] : null);
                PintarEvento(4, eventos.Count > 4 ? eventos[4] : null);
            }
            catch
            {
                MostrarUltimosEventosEnError();
            }
        }

        private void MostrarUltimosEventosEnError()
        {
            lblFacturacion.Text = "No se pudieron cargar los eventos recientes.";
            StatusFactura.Text = "";

            lblDunning.Text = "";
            StatusDunning.Text = "";

            lblPagos.Text = "";
            StatusPagos.Text = "";

            lblValidacion.Text = "";
            StatusValidacion.Text = "";

            lblInicioMes.Text = "";
            StatusInicioMes.Text = "";
        }

        private void PintarEvento(int indice, DashboardEvento eventoSistema)
        {
            Label descripcion = null;
            Label fecha = null;

            switch (indice)
            {
                case 0:
                    descripcion = lblFacturacion;
                    fecha = StatusFactura;
                    break;

                case 1:
                    descripcion = lblDunning;
                    fecha = StatusDunning;
                    break;

                case 2:
                    descripcion = lblPagos;
                    fecha = StatusPagos;
                    break;

                case 3:
                    descripcion = lblValidacion;
                    fecha = StatusValidacion;
                    break;

                case 4:
                    descripcion = lblInicioMes;
                    fecha = StatusInicioMes;
                    break;
            }

            if (descripcion == null || fecha == null)
                return;

            if (eventoSistema == null)
            {
                descripcion.Text = "Sin actividad reciente.";
                fecha.Text = "";
                return;
            }

            descripcion.Text = eventoSistema.Accion + " - " + eventoSistema.Modulo;
            fecha.Text = eventoSistema.Fecha.ToString("dd/MM/yyyy");
        }

        #endregion

        #region 09 - Navegación

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            CargarDashboard();
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

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionHelper.LogoutGlobal();
        }

        #endregion

        #region 10 - Utilidades visuales

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

        private void MostrarLineaEvento(
            KryptonBorderEdge borde,
            Label descripcion,
            Label fecha)
        {
            if (borde != null)
                borde.Visible = true;

            if (descripcion != null)
            {
                descripcion.Visible = true;
                descripcion.TextAlign = ContentAlignment.MiddleLeft;
            }

            if (fecha != null)
            {
                fecha.Visible = true;
                fecha.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        #endregion
    }
}