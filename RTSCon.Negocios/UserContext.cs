using System;

namespace RTSCon.Negocios.Auth
{
    public static class UserContext
    {
        public static int UsuarioAuthId { get; private set; }
        public static string Usuario { get; private set; }
        public static string Rol { get; private set; }
        public static DateTime InicioSesionUtc { get; private set; }
        public static DateTime UltimaActividadUtc { get; private set; }

        public static void Set(int id, string usuario, string rol)
        {
            UsuarioAuthId = id;
            Usuario = usuario;
            Rol = rol;
            InicioSesionUtc = DateTime.UtcNow;
            UltimaActividadUtc = InicioSesionUtc;
        }

        public static void Clear()
        {
            UsuarioAuthId = 0;
            Usuario = null;
            Rol = null;
            InicioSesionUtc = DateTime.MinValue;
            UltimaActividadUtc = DateTime.MinValue;
        }

        public static void Touch()
        {
            if (UsuarioAuthId != 0)
                UltimaActividadUtc = DateTime.UtcNow;
        }

        public static bool IsLoggedIn => UsuarioAuthId != 0;

        public static bool EsSA =>
            string.Equals(Rol, "SA", StringComparison.OrdinalIgnoreCase);

        public static bool EsPropietarioActual =>
            string.Equals(Rol, "Propietario", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Rol, "Admin", StringComparison.OrdinalIgnoreCase);
    }
}
