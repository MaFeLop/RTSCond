using System;

namespace RTSCon.Negocios
{
    /// <summary>
    /// Contexto global de sesión autenticada.
    /// </summary>
    public static class AuthSession
    {
        // =========================
        // Identidad
        // =========================
        public static int UsuarioAuthId { get; private set; }
        public static string Usuario { get; private set; }
        public static string Rol { get; private set; }

        // =========================
        // Estado
        // =========================
        public static bool IsLoggedIn => UsuarioAuthId > 0;

        // =========================
        // Login
        // =========================
        public static void Start(int usuarioAuthId, string usuario, string rol)
        {
            UsuarioAuthId = usuarioAuthId;
            Usuario = usuario;
            Rol = rol;
        }

        // =========================
        // Logout
        // =========================
        public static void Clear()
        {
            UsuarioAuthId = 0;
            Usuario = null;
            Rol = null;
        }

        // =========================
        // Helpers de rol
        // =========================
        public static bool EsSA =>
            string.Equals(Rol, "SA", StringComparison.OrdinalIgnoreCase);

        public static bool EsPropietario =>
            string.Equals(Rol, "Propietario", StringComparison.OrdinalIgnoreCase);

        public static bool EsSecretario =>
            string.Equals(Rol, "Secretario", StringComparison.OrdinalIgnoreCase);

        public static bool EsInquilino =>
            string.Equals(Rol, "Inquilino", StringComparison.OrdinalIgnoreCase);
    }
}
