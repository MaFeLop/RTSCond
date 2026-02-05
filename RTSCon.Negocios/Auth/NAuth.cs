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
            UltimaActividadUtc = DateTime.UtcNow;
        }

        public static void Touch() => UltimaActividadUtc = DateTime.UtcNow;

        public static void Clear()
        {
            UsuarioAuthId = 0;
            Usuario = null;
            Rol = null;
            UltimaActividadUtc = DateTime.MinValue;
        }

        public static bool EsPropietarioActual
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Rol))
                    return false;

                return Rol.Equals("PROPIETARIO", StringComparison.OrdinalIgnoreCase);
            }
        }

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

        // ============================
        // LOGIN PASSWORD
        // ============================
        public int Login_Password(
            string usuario,
            string password,
            string mailProfile,
            int minutosCodigo,
            bool debug
        )
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Contraseña requerida.");

            DataRow row = _dal.ObtenerPorUsuario(usuario.Trim());

            if (row == null)
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            string hash = Convert.ToString(row["pass_hash"]);

            if (!BCrypt.Net.BCrypt.Verify(password, hash))
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            int id = Convert.ToInt32(row["ID_usr"]);
            string rol = Convert.ToString(row["Rol"]);

            UserContext.Set(id, usuario.Trim(), rol);
            _dal.MarcarLogin(id);

            return id;
        }

        // ============================
        // LOGIN 2FA (PLACEHOLDER)
        // ============================
        public bool Login_CodigoYSesion(int usuarioAuthId, string codigo)
        {
            if (usuarioAuthId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            // Placeholder hasta conectar 2FA real
            UserContext.Touch();
            return true;
        }


        public bool Login_Codigo(int usuarioAuthId, string codigo)
        {
            if (usuarioAuthId <= 0) return false;
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 6) return false;

            UserContext.Touch();
            return true;
        }

        // ============================
        // REENVÍO CÓDIGO 2FA
        // ============================
        public void ReenviarCodigo(
            int usuarioAuthId,
            string mailProfile,
            int minutos,
            bool debug
        )
        {
            if (usuarioAuthId <= 0)
                throw new ArgumentOutOfRangeException(nameof(usuarioAuthId));

            // Placeholder estable
        }

        // ============================
        // RECUPERACIÓN CONTRASEÑA
        // ============================
        public int EnviarCodigoRecuperacion(
            string usuario,
            string correo,
            string mailProfile,
            int minutos,
            bool debug
        )
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            DataRow row = _dal.ObtenerPorUsuario(usuario.Trim());
            if (row == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            return Convert.ToInt32(row["ID_usr"]);
        }

        public void CambiarPasswordConCodigo(
            int usuarioAuthId,
            string codigo,
            string nuevaPassword,
            string editor,
            int iteraciones
        )
        {
            if (usuarioAuthId <= 0)
                throw new ArgumentOutOfRangeException(nameof(usuarioAuthId));

            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código requerido.");

            if (string.IsNullOrWhiteSpace(nuevaPassword))
                throw new ArgumentException("Nueva contraseña requerida.");

            UserContext.Touch();
        }

        // ============================
        // CREAR USUARIO
        // ============================
        public int CrearUsuario(
            string usuario,
            string correo,
            string password,
            string rol,
            string creador
        )
        {
            int idRol =
                rol.Equals("SA", StringComparison.OrdinalIgnoreCase) ? 1 :
                rol.Equals("Admin", StringComparison.OrdinalIgnoreCase) ? 2 :
                rol.Equals("Propietario", StringComparison.OrdinalIgnoreCase) ? 3 :
                rol.Equals("Inquilino", StringComparison.OrdinalIgnoreCase) ? 4 :
                throw new ArgumentException("Rol inválido.");

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            return _dal.CrearUsuario(
                usuario.Trim(),
                correo?.Trim(),
                hash,
                idRol
            );
        }

        public bool ValidarPassword(int idUsuario, string password)
        {
            if (idUsuario <= 0 || string.IsNullOrWhiteSpace(password))
                return false;

            DataRow row = _dal.ObtenerPorId(idUsuario);
            if (row == null) return false;

            string hash = Convert.ToString(row["hash_bcrypt"]);
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }


    }
}
