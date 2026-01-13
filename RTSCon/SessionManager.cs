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
        private static Action _onTimeout;

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

                    // Marca actividad en tu contexto
                    UserContext.Touch();
                }
                return false;
            }
        }

        public static void Start(Form owner, int idleMinutes = 15, int promptMinutes = 13, Action onTimeout = null)
        {
            _owner = owner; // puede ser null
            _onTimeout = onTimeout;

            _idleMinutes = Math.Max(1, idleMinutes);
            _promptMinutes = Math.Max(0, Math.Min(promptMinutes, _idleMinutes));

            _lastActivityUtc = DateTime.UtcNow;
            _promptShown = false;

            Application.AddMessageFilter(new ActivityFilter());

            // Si el owner se cierra, paramos el manager para que no intente usarlo luego
            if (_owner != null)
            {
                _owner.FormClosed -= Owner_FormClosed;
                _owner.FormClosed += Owner_FormClosed;
            }

            if (_timer == null)
            {
                _timer = new Timer { Interval = 1000 };
                _timer.Tick += Timer_Tick;
            }

            _timer.Start();
        }

        private static void Owner_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Owner murió: deja de apuntar a él y detén el timer
            _owner = null;
            Stop();
        }

        public static void Stop()
        {
            if (_timer == null) return;

            _timer.Stop();
            _timer.Tick -= Timer_Tick;
            _timer.Dispose();
            _timer = null;

            _promptShown = false;
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            // Si no hay sesión, no hacemos nada
            if (UserContext.UsuarioAuthId == 0) return;

            var minutes = (DateTime.UtcNow - _lastActivityUtc).TotalMinutes;

            // Ventana de aviso
            if (!_promptShown && _promptMinutes > 0 && minutes >= _promptMinutes && minutes < _idleMinutes)
            {
                _promptShown = true;

                var remaining = TimeSpan.FromMinutes(_idleMinutes) - (DateTime.UtcNow - _lastActivityUtc);
                var secs = Math.Max(0, (int)remaining.TotalSeconds);

                var r = SafeMessageBoxYesNo(
                    $"No se detecta actividad.\n¿Desea mantener su sesión?\nTiempo restante: {secs} s.",
                    "Sesión inactiva"
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

            // Timeout
            if (minutes >= _idleMinutes)
                DoTimeout();
        }

        private static void DoTimeout()
        {
            Stop();

            // Mensaje seguro (con o sin owner)
            SafeMessageBoxOk("Su sesión expiró por inactividad.", "Sesión expirada");

            try
            {
                // Marca que estamos cerrando sesión para evitar re-show de hubs
                SessionHelper.BeginLogout();

                // Limpieza de contexto
                UserContext.Clear();

                // Ejecuta callback si te interesa (ej. abrir login)
                _onTimeout?.Invoke();
            }
            catch { /* swallow */ }

            // Logout global (cierra forms y abre Login)
            SessionHelper.LogoutGlobal();
        }

        private static DialogResult SafeMessageBoxYesNo(string message, string title)
        {
            try
            {
                var owner = GetSafeOwner();
                if (owner != null)
                {
                    return KryptonMessageBox.Show(owner, message, title,
                        KryptonMessageBoxButtons.YesNo,
                        KryptonMessageBoxIcon.Information);
                }

                return KryptonMessageBox.Show(message, title,
                    KryptonMessageBoxButtons.YesNo,
                    KryptonMessageBoxIcon.Information);
            }
            catch
            {
                // Si incluso así falla, no bloqueamos flujo
                return DialogResult.No;
            }
        }

        private static void SafeMessageBoxOk(string message, string title)
        {
            try
            {
                var owner = GetSafeOwner();
                if (owner != null)
                {
                    KryptonMessageBox.Show(owner, message, title,
                        KryptonMessageBoxButtons.OK,
                        KryptonMessageBoxIcon.Information);
                    return;
                }

                KryptonMessageBox.Show(message, title,
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
            }
            catch { /* swallow */ }
        }

        private static Form GetSafeOwner()
        {
            // Si _owner está vivo, úsalo
            if (_owner != null && !_owner.IsDisposed && _owner.IsHandleCreated && _owner.Visible)
                return _owner;

            // Si no, busca algún form visible
            try
            {
                foreach (Form f in Application.OpenForms)
                {
                    if (f != null && !f.IsDisposed && f.IsHandleCreated && f.Visible)
                        return f;
                }
            }
            catch { }

            return null;
        }
    }
}
