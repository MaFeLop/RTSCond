using System;
using System.Data;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
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

        public static bool EsPropietarioActual =>
            string.Equals(Rol, "PROPIETARIO", StringComparison.OrdinalIgnoreCase);
    }

    public class NAuth
    {
        private readonly DAuth _dal;

        public NAuth(DAuth dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // ================= LOGIN =================
        public int Login(string usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Contraseña requerida.");

            DataRow row = _dal.Login(usuario.Trim(), password.Trim());

            if (row == null)
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            int id = Convert.ToInt32(row["ID_usr"]);
            string rol = Convert.ToString(row["Rol"]);

            UserContext.Set(id, usuario.Trim(), rol);
            _dal.MarcarLogin(id);

            return id;
        }

        // ================= VALIDAR PASSWORD =================
        public bool ValidarPassword(int usuarioId, string password)
        {
            if (usuarioId <= 0 || string.IsNullOrWhiteSpace(password))
                return false;

            return _dal.ValidarPassword(usuarioId, password);
        }

        // ================= CREAR USUARIO =================
        public int CrearUsuario(string usuario, string correo, string password, string rol, string creador)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Contraseña requerida.");

            return _dal.CrearUsuario(usuario.Trim(), correo?.Trim(), password, rol);
        }

        // ================= RECUPERACIÓN =================
        public int EnviarCodigoRecuperacion(string usuario, string correo)
        {
            return _dal.EnviarCodigoRecuperacion(usuario, correo);
        }

        public bool ValidarCodigoRecuperacion(int usuarioId, string codigo)
        {
            return _dal.ValidarCodigoRecuperacion(usuarioId, codigo);
        }

        public void CambiarPasswordConCodigo(int usuarioId, string codigo, string nuevaPassword)
        {
            _dal.CambiarPasswordConCodigo(usuarioId, codigo, nuevaPassword);
        }
    }
}

