using System;
using System.Linq;
using System.Windows.Forms;
using RTSCon.Negocios;

namespace RTSCon
{
    public static class SessionHelper
    {
        // =========================
        // Identidad de sesión
        // =========================
        public static string Usuario { get; private set; }
        public static string Rol { get; private set; }
        public static int? UsuarioId { get; private set; }

        // =========================
        // Estado de logout
        // =========================
        public static bool IsLoggingOut { get; private set; }

        // =========================
        // Inicio de sesión
        // =========================
        public static void Start(string usuario, string rol, int? usuarioId = null)
        {
            Usuario = usuario;
            Rol = rol;
            UsuarioId = usuarioId;
            IsLoggingOut = false;
        }

        // =========================
        // Logout controlado
        // =========================
        public static void BeginLogout() => IsLoggingOut = true;
        public static void EndLogout() => IsLoggingOut = false;

        public static void LogoutFrom(Form current)
        {
            if (current == null) return;

            BeginLogout();

            try { UserContext.Clear(); } catch { }

            try { current.Close(); } catch { }
        }

        public static void LogoutGlobal()
        {
            BeginLogout();

            try { UserContext.Clear(); } catch { }

            // Abrir Login primero
            try
            {
                var login = Application.OpenForms
                    .Cast<Form>()
                    .FirstOrDefault(f => f is Login);

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
            catch { }

            // Cerrar todo lo demás
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
                    catch { }
                }
            }
            catch { }
        }

        // =========================
        // Helpers de sesión
        // =========================
        public static bool IsLogged =>
            !string.IsNullOrWhiteSpace(Usuario);

        // =========================
        // Helpers de rol (jerarquía)
        // =========================
        public static bool IsSA =>
            string.Equals(Rol, "SA", StringComparison.OrdinalIgnoreCase);

        public static bool IsPropietario =>
            string.Equals(Rol, "Propietario", StringComparison.OrdinalIgnoreCase);

        public static bool IsSecretario =>
            string.Equals(Rol, "Secretario", StringComparison.OrdinalIgnoreCase);

        public static bool IsInquilino =>
            string.Equals(Rol, "Inquilino", StringComparison.OrdinalIgnoreCase);

        // =========================
        // Permisos por jerarquía
        // =========================
        public static bool CanCreateSA =>
            IsSA;

        public static bool CanCreatePropietario =>
            IsSA;

        public static bool CanCreateSecretario =>
            IsSA || IsPropietario;

        public static bool CanCreateInquilino =>
            IsSA || IsPropietario || IsSecretario;
    }
}
