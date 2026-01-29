using System;
using System.Data;
using BCrypt.Net;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    // ============================
    // CONTEXTO DE USUARIO (ÚNICO)
    // ============================
    public static class UserContext
    {
        public static int UsuarioAuthId { get; private set; }
        public static string Usuario { get; private set; }
        public static string Rol { get; private set; }

        public static DateTime UltimaActividadUtc { get; private set; }

        public static bool IsLoggedIn => UsuarioAuthId > 0;

        public static void Set(int id, string usuario, string rol)
        {
            UsuarioAuthId = id;
            Usuario = usuario;
            Rol = rol;
            Touch();
        }

        public static void Touch()
        {
            UltimaActividadUtc = DateTime.UtcNow;
        }

        public static void Clear()
        {
            UsuarioAuthId = 0;
            Usuario = null;
            Rol = null;
            UltimaActividadUtc = DateTime.MinValue;
        }

        // ===== Helpers usados por la UI =====
        public static bool EsSA =>
            string.Equals(Rol, "SA", StringComparison.OrdinalIgnoreCase);

        public static bool EsPropietario =>
            string.Equals(Rol, "Propietario", StringComparison.OrdinalIgnoreCase);

        public static bool EsPropietarioActual =>
            EsPropietario && UsuarioAuthId > 0;
    }


    // ============================
    // AUTH
    // ============================
    public class NAuth
    {
        private readonly DAuth _dal;

        public NAuth(DAuth dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // ----------------------------
        // CREAR USUARIO
        // ----------------------------
        public int CrearUsuario(string usuario, string correo, string password, int idRol)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Contraseña requerida.");

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            return _dal.CrearUsuario(
                usuario.Trim(),
                correo?.Trim(),
                hash,
                idRol
            );
        }

        // ----------------------------
        // LOGIN
        // ----------------------------
        public void Login(string usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Contraseña requerida.");

            DataRow row = _dal.ObtenerPorUsuario(usuario.Trim());

            if (row == null)
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            string hash = Convert.ToString(row["hash_bcrypt"]);

            if (!BCrypt.Net.BCrypt.Verify(password, hash))
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            int idUsuario = Convert.ToInt32(row["ID_usr"]);

            // ⚠️ IMPORTANTE:
            // El SP debe devolver el NOMBRE del rol, NO el ID
            string rol = Convert.ToString(row["Rol"]);

            UserContext.Set(idUsuario, usuario.Trim(), rol);

            _dal.MarcarLogin(idUsuario);
        }

        // ----------------------------
        // VALIDAR PASSWORD
        // ----------------------------
        public bool ValidarPassword(int idUsuario, string password)
        {
            if (idUsuario <= 0 || string.IsNullOrWhiteSpace(password))
                return false;

            if (!UserContext.IsLoggedIn)
                return false;

            DataRow row = _dal.ObtenerPorUsuario(UserContext.Usuario);

            if (row == null)
                return false;

            string hash = Convert.ToString(row["hash_bcrypt"]);
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public void Login_Password(string usuario, string password)
        {
            Login(usuario, password);
        }

        public void Login_Codigo(string usuario, string codigo)
        {
            // Placeholder hasta que conectes 2FA real
            throw new NotImplementedException("Login por código no implementado aún.");
        }

        public void Login_CodigoYSesion(string usuario, string codigo)
        {
            Login_Codigo(usuario, codigo);
        }

        public void ReenviarCodigo(string usuario)
        {
            // Placeholder controlado
        }

        public void EnviarCodigoRecuperacion(string usuario)
        {
            // Placeholder
        }

        public string CorreoEnmascarado(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return string.Empty;
            int at = correo.IndexOf('@');
            if (at <= 2) return "***" + correo.Substring(at);
            return correo.Substring(0, 2) + "***" + correo.Substring(at);
        }

        public void CambiarPasswordConCodigo(string usuario, string codigo, string nuevoPassword)
        {
            // Placeholder
        }

    }
}
