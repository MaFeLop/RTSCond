using System;
using System.Configuration;
using System.Windows.Forms;
using Krypton.Toolkit;
using RTSCon.Negocios;

namespace RTSCon
{
    /// <summary>
    /// Comportamiento de timeout de sesión que se "inyecta" en cualquier Form.
    /// No cambia la clase base (sigues usando KryptonForm).
    /// </summary>
    public class SessionTimeoutBehavior : IDisposable
    {
        private readonly Form _form;
        private readonly Timer _timer;
        private readonly int _timeoutMinutes;

        public SessionTimeoutBehavior(Form form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));

            _timeoutMinutes = int.TryParse(
                ConfigurationManager.AppSettings["SessionTimeoutMinutes"],
                out var m) ? m : 15;  // por defecto 15 minutos

            _timer = new Timer { Interval = 30_000 }; // chequeamos cada 30 segundos
            _timer.Tick += Timer_Tick;

            _form.MouseMove += ActivityDetected;
            _form.KeyDown += ActivityDetected;
            _form.FormClosed += FormClosed;

            _timer.Start();
        }

        private void ActivityDetected(object sender, EventArgs e)
        {
            UserContext.Touch();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (UserContext.UsuarioAuthId == 0)
                return; // nadie logueado

            var inactivo = DateTime.UtcNow - UserContext.UltimaActividadUtc;
            if (inactivo > TimeSpan.FromMinutes(_timeoutMinutes))
            {
                _timer.Stop();

                KryptonMessageBox.Show(
                    _form,
                    "Su sesión ha expirado por inactividad.",
                    "Sesión expirada",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);

                SessionHelper.LogoutGlobal();
            }
        }

        private void FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;

            _form.MouseMove -= ActivityDetected;
            _form.KeyDown -= ActivityDetected;
            _form.FormClosed -= FormClosed;
        }
    }
}
