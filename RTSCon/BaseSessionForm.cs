using System;
using System.Configuration;
using System.Windows.Forms;
using Krypton.Toolkit;
using RTSCon.Negocios;

namespace RTSCon
{
    /// <summary>
    /// Formulario base que agrega control de sesión:
    /// - Registra actividad de teclado/ratón.
    /// - Verifica inactividad periódicamente.
    /// - Si se excede el tiempo, cierra sesión globalmente.
    /// 
    /// Todos los formularios protegidos (Dashboard, catálogos, etc.)
    /// deben heredar de BaseSessionForm en vez de KryptonForm.
    /// </summary>
    public class BaseSessionForm : KryptonForm
    {
        private readonly Timer _sessionTimer = new Timer();

        public BaseSessionForm()
        {
            // Intervalo de chequeo: cada 30 segundos
            _sessionTimer.Interval = 30_000;
            _sessionTimer.Tick += SessionTimer_Tick;

            // Eventos de actividad
            this.MouseMove += ActivityDetected;
            this.KeyDown += ActivityDetected;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _sessionTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            _sessionTimer.Stop();
            _sessionTimer.Tick -= SessionTimer_Tick;

            this.MouseMove -= ActivityDetected;
            this.KeyDown -= ActivityDetected;
        }

        private void ActivityDetected(object sender, EventArgs e)
        {
            UserContext.Touch();  // actualiza UltimaActividadUtc
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            // Si no hay usuario logueado, no hacemos nada
            if (UserContext.UsuarioAuthId == 0)
                return;

            // Minutos de timeout desde App.config
            int timeoutMinutes = int.TryParse(
                ConfigurationManager.AppSettings["SessionTimeoutMinutes"],
                out var m) ? m : 3; // por defecto 15 minutos

            var inactivo = DateTime.UtcNow - UserContext.UltimaActividadUtc;
            if (inactivo > TimeSpan.FromMinutes(timeoutMinutes))
            {
                _sessionTimer.Stop();

                KryptonMessageBox.Show(
                    this,
                    "Su sesión ha expirado por inactividad.",
                    "Sesión expirada",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);

                SessionHelper.LogoutGlobal();
            }
        }
    }
}
