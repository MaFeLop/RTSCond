using System;
using System.Linq;
using System.Windows.Forms;
using RTSCon.Negocios;

namespace RTSCon
{
    public static class SessionHelper
    {
        public static string Usuario { get; private set; }
        public static string Rol { get; private set; }
        public static int? UsuarioId { get; private set; }

        public static bool IsLoggingOut { get; private set; }

        public static void Start(string usuario, string rol, int? usuarioId = null)
        {
            Usuario = usuario;
            Rol = rol;
            UsuarioId = usuarioId;
            IsLoggingOut = false;
        }

        private static void ClearIdentity()
        {
            Usuario = null;
            Rol = null;
            UsuarioId = null;
        }

        public static void Clear()
        {
            ClearIdentity();
            IsLoggingOut = false;
        }

        public static void BeginLogout()
        {
            IsLoggingOut = true;
        }

        public static void EndLogout()
        {
            IsLoggingOut = false;
        }

        public static void LogoutFrom(Form current)
        {
            LogoutGlobal();
        }

        public static void LogoutGlobal()
        {
            LogoutGlobalCore(null);
        }

        public static void LogoutGlobalPorInactividad()
        {
            LogoutGlobalCore("Su sesión fue automáticamente cerrada por inactividad. Vuelva a iniciar sesión.");
        }

        private static void LogoutGlobalCore(string mensaje)
        {
            if (IsLoggingOut)
                return;

            BeginLogout();

            try { SessionManager.Stop(); } catch { }
            try { UserContext.Clear(); } catch { }

            ClearIdentity();

            Form login = null;

            try
            {
                login = Application.OpenForms
                    .Cast<Form>()
                    .FirstOrDefault(delegate (Form f)
                    {
                        return f is Login && !f.IsDisposed;
                    });
            }
            catch
            {
                login = null;
            }

            try
            {
                Form[] forms = Application.OpenForms.Cast<Form>().ToArray();

                foreach (Form f in forms)
                {
                    if (f is Login)
                        continue;

                    try { f.Close(); } catch { }
                }
            }
            catch
            {
            }

            try
            {
                if (login == null || login.IsDisposed)
                    login = new Login();

                if (!login.Visible)
                    login.Show();

                login.Activate();
                login.BringToFront();
            }
            catch
            {
            }

            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                try
                {
                    Form owner = login;

                    if (owner != null && owner.IsHandleCreated)
                    {
                        owner.BeginInvoke((Action)delegate
                        {
                            MostrarAvisoSesionExpirada(owner, mensaje);
                        });
                    }
                    else
                    {
                        MostrarAvisoSesionExpirada(login, mensaje);
                    }

                    return;
                }
                catch
                {
                }
            }

            EndLogout();
        }

        private static void MostrarAvisoSesionExpirada(Form owner, string mensaje)
        {
            try
            {
                using (FrmSesionExpirada frm = new FrmSesionExpirada(mensaje))
                {
                    frm.ShowDialog(owner);
                }
            }
            finally
            {
                EndLogout();
            }
        }

        public static bool IsLogged
        {
            get { return !string.IsNullOrWhiteSpace(Usuario); }
        }

        public static bool IsSA
        {
            get { return string.Equals(Rol, "SA", StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsPropietario
        {
            get { return string.Equals(Rol, "Propietario", StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsSecretario
        {
            get { return string.Equals(Rol, "Secretario", StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsInquilino
        {
            get { return string.Equals(Rol, "Inquilino", StringComparison.OrdinalIgnoreCase); }
        }

        public static bool CanCreateSA
        {
            get { return IsSA; }
        }

        public static bool CanCreatePropietario
        {
            get { return IsSA; }
        }

        public static bool CanCreateSecretario
        {
            get { return IsSA || IsPropietario; }
        }

        public static bool CanCreateInquilino
        {
            get { return IsSA || IsPropietario || IsSecretario; }
        }
    }
}