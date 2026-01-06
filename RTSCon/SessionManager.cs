using System;
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

        private sealed class ActivityFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                const int WM_MOUSEMOVE = 0x0200;
                const int WM_KEYDOWN = 0x0100;
                const int WM_LBUTTON = 0x0201;
                const int WM_RBUTTON = 0x0204;
                const int WM_MBUTTON = 0x0207;

                if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_KEYDOWN ||
                    m.Msg == WM_LBUTTON || m.Msg == WM_RBUTTON || m.Msg == WM_MBUTTON)
                {
                    _lastActivityUtc = DateTime.UtcNow;
                    _promptShown = false;
                    UserContext.Touch();
                }
                return false;
            }
        }

        public static void Start(Form owner, int idleMinutes = 15, int promptMinutes = 13)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _idleMinutes = Math.Max(1, idleMinutes);
            _promptMinutes = Math.Max(0, Math.Min(promptMinutes, _idleMinutes));

            _lastActivityUtc = DateTime.UtcNow;
            _promptShown = false;

            Application.AddMessageFilter(new ActivityFilter());

            _timer = new Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public static void Stop()
        {
            if (_timer == null) return;

            _timer.Stop();
            _timer.Tick -= Timer_Tick;
            _timer.Dispose();
            _timer = null;
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            // ✅ AQUÍ ESTABA EL ERROR (IsLoggedIn ya existe)
            if (!UserContext.IsLoggedIn)
                return;

            var minutes = (DateTime.UtcNow - _lastActivityUtc).TotalMinutes;

            if (!_promptShown &&
                _promptMinutes > 0 &&
                minutes >= _promptMinutes &&
                minutes < _idleMinutes)
            {
                _promptShown = true;

                var remaining = TimeSpan.FromMinutes(_idleMinutes) -
                                (DateTime.UtcNow - _lastActivityUtc);

                var secs = Math.Max(0, (int)remaining.TotalSeconds);

                var r = KryptonMessageBox.Show(
                    _owner,
                    $"No se detecta actividad.\n¿Desea mantener su sesión?\nTiempo restante: {secs} s.",
                    "Sesión inactiva",
                    KryptonMessageBoxButtons.YesNo,
                    KryptonMessageBoxIcon.Information
                );

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
                _owner,
                "Su sesión expiró por inactividad.",
                "Sesión expirada",
                KryptonMessageBoxButtons.OK,
                KryptonMessageBoxIcon.Information
            );

            SessionHelper.LogoutGlobal();
        }
    }
}
