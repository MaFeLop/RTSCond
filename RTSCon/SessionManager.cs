using System;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace RTSCon
{
    /// <summary>
    /// Captura actividad de teclado/mouse y cierra sesión por inactividad.
    /// </summary>
    public static class SessionManager
    {
        private static DateTime _lastActivityUtc = DateTime.UtcNow;
        private static Timer _timer;
        private static bool _promptShown;
        private static int _idleMinutes;
        private static int _promptMinutes;
        private static Form _owner;
        private static Action _onTimeout;

        private sealed class ActivityFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                // Casi todo: teclado y mouse
                const int WM_MOUSEMOVE = 0x0200;
                const int WM_KEYDOWN = 0x0100;
                const int WM_LBUTTON = 0x0201;
                const int WM_RBUTTON = 0x0204;
                const int WM_MBUTTON = 0x0207;

                if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_KEYDOWN ||
                    m.Msg == WM_LBUTTON || m.Msg == WM_RBUTTON || m.Msg == WM_MBUTTON)
                {
                    _lastActivityUtc = DateTime.UtcNow;
                    _promptShown = false; // si hubo actividad, olvidamos el prompt
                }
                return false;
            }
        }

        public static void Start(Form owner, int idleMinutes, int promptMinutes, Action onTimeout)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _idleMinutes = Math.Max(1, idleMinutes);
            _promptMinutes = Math.Min(_idleMinutes, Math.Max(0, promptMinutes));
            _onTimeout = onTimeout ?? throw new ArgumentNullException(nameof(onTimeout));

            _lastActivityUtc = DateTime.UtcNow;
            _promptShown = false;

            // Filtro de actividad global
            Application.AddMessageFilter(new ActivityFilter());

            // Timer 1s
            _timer = new Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public static void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick;
                _timer.Dispose();
                _timer = null;
            }
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            var minutes = (DateTime.UtcNow - _lastActivityUtc).TotalMinutes;

            // Mostrar aviso una sola vez al llegar a la ventana de aviso
            if (!_promptShown && minutes >= _promptMinutes && minutes < _idleMinutes)
            {
                _promptShown = true;
                var remaining = TimeSpan.FromMinutes(_idleMinutes) - (DateTime.UtcNow - _lastActivityUtc);
                var msg = $"No se detecta actividad.\n¿Desea mantener su sesión?\n" +
                          $"Tiempo restante: {Math.Max(0, (int)remaining.TotalSeconds)} s.";

                var r = KryptonMessageBox.Show(_owner, msg, "Sesión inactiva",
                        KryptonMessageBoxButtons.YesNo, KryptonMessageBoxIcon.Information);

                if (r == DialogResult.Yes)
                {
                    _lastActivityUtc = DateTime.UtcNow; // renovar sesión
                    _promptShown = false;
                    return;
                }
                // Si dice No, cerramos ya
                DoTimeout();
                return;
            }

            // Hora de cerrar sesión
            if (minutes >= _idleMinutes)
            {
                DoTimeout();
            }
        }

        private static void DoTimeout()
        {
            Stop();
            try
            {
                _onTimeout?.Invoke();
            }
            catch { /* swallow */ }
        }
    }
}
