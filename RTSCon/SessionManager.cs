using System;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using Krypton.Toolkit;
using RTSCon.Negocios;

namespace RTSCon
{
    public static class SessionManager
    {
        private static DateTime _lastActivityUtc;
        private static Timer _timer;
        private static bool _promptShown;

        private static int _idleMinutes;
        private static int _promptMinutes;

        private static Form _owner;
        private static Action _onTimeout;

        private static ActivityFilter _filter; // 🔥 IMPORTANTE

        private sealed class ActivityFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                const int WM_MOUSEMOVE = 0x0200;
                const int WM_KEYDOWN = 0x0100;
                const int WM_LBUTTON = 0x0201;
                const int WM_RBUTTON = 0x0204;
                const int WM_MBUTTON = 0x0207;

                if (m.Msg == WM_MOUSEMOVE ||
                    m.Msg == WM_KEYDOWN ||
                    m.Msg == WM_LBUTTON ||
                    m.Msg == WM_RBUTTON ||
                    m.Msg == WM_MBUTTON)
                {
                    _lastActivityUtc = DateTime.UtcNow;
                    _promptShown = false;
                    UserContext.Touch();
                }

                return false;
            }
        }

        public static void Start(Form owner, int idleMinutes, int promptMinutes, Action onTimeout = null)
        {
            _owner = owner;
            _onTimeout = onTimeout;

            _idleMinutes = Math.Max(1, idleMinutes);
            _promptMinutes = Math.Max(0, Math.Min(promptMinutes, _idleMinutes));

            _lastActivityUtc = DateTime.UtcNow;
            _promptShown = false;

            // 🔥 SOLO AGREGAR FILTRO SI NO EXISTE
            if (_filter == null)
            {
                _filter = new ActivityFilter();
                Application.AddMessageFilter(_filter);
            }

            if (_timer == null)
            {
                _timer = new Timer { Interval = 1000 };
                _timer.Tick += Timer_Tick;
            }

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

            if (_filter != null)
            {
                Application.RemoveMessageFilter(_filter); // 🔥 CLAVE
                _filter = null;
            }

            _promptShown = false;
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            if (UserContext.UsuarioAuthId == 0)
                return;

            var minutes = (DateTime.UtcNow - _lastActivityUtc).TotalMinutes;

            if (!_promptShown &&
                _promptMinutes > 0 &&
                minutes >= _promptMinutes &&
                minutes < _idleMinutes)
            {
                _promptShown = true;

                var remaining = TimeSpan.FromMinutes(_idleMinutes) - (DateTime.UtcNow - _lastActivityUtc);
                var secs = Math.Max(0, (int)remaining.TotalSeconds);

                var r = KryptonMessageBox.Show(
                    $"No se detecta actividad.\n¿Desea mantener su sesión?\nTiempo restante: {secs} s.",
                    "Sesión inactiva",
                    KryptonMessageBoxButtons.YesNo,
                    KryptonMessageBoxIcon.Information);

                if (r == DialogResult.Yes)
                {
                    _lastActivityUtc = DateTime.UtcNow;
                    _promptShown = false;
                    UserContext.Touch();
                    return;
                }

                DoTimeout();
                return;
            }

            if (minutes >= _idleMinutes)
                DoTimeout();
        }

        private static void DoTimeout()
        {
            Stop();

            KryptonMessageBox.Show(
                "Su sesión expiró por inactividad.",
                "Sesión expirada",
                KryptonMessageBoxButtons.OK,
                KryptonMessageBoxIcon.Information);

            SessionHelper.LogoutGlobal();
        }
    }
}
