using System;
using System.Linq;
using System.Windows.Forms;
using RTSCon.Negocios;

namespace RTSCon
{
    public static class SessionHelper
    {
        // Solo se puede cambiar desde aquí (no desde fuera)
        public static bool IsLoggingOut { get; private set; }

        public static void BeginLogout() => IsLoggingOut = true;
        public static void EndLogout() => IsLoggingOut = false;

        public static void LogoutFrom(Form current)
        {
            if (current == null) return;

            BeginLogout();

            try { UserContext.Clear(); } catch { }

            try
            {
                var login = new Login();
                login.Show();
            }
            catch { /* si falla, seguimos cerrando */ }

            try { current.Close(); } catch { }
        }

        public static void LogoutGlobal()
        {
            BeginLogout();

            try { UserContext.Clear(); } catch { }

            // Abre login primero
            try
            {
                var login = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f is Login);
                if (login == null || login.IsDisposed)
                {
                    login = new Login();
                    login.Show();
                }
                else
                {
                    login.Show();
                    login.Activate();
                }
            }
            catch { /* swallow */ }

            // Cierra todo lo demás
            try
            {
                var abiertos = Application.OpenForms.Cast<Form>().ToList();
                foreach (var f in abiertos)
                {
                    if (f == null) continue;
                    if (f is Login) continue;

                    try
                    {
                        if (!f.IsDisposed)
                            f.Close();
                    }
                    catch { /* swallow */ }
                }
            }
            catch { /* swallow */ }
        }
    }
}
