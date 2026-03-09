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

            DataRow row = _dal.Login(usuario.Trim(), password);

            if (row == null)
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            int id = Convert.ToInt32(row["Id"]);
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

            int idRol = _dal.ObtenerIdRol(rol);

            if (idRol == 0)
                throw new InvalidOperationException("Rol inválido.");

            return _dal.CrearUsuario(usuario.Trim(), correo?.Trim(), password, idRol);
        }

        // ================= RECUPERACIÓN / CÓDIGO =================
        public int ObtenerUsuarioAuthIdPorUsuarioYCorreo(string usuario, string correo)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("Correo requerido.");

            return _dal.ObtenerUsuarioAuthIdPorUsuarioYCorreo(usuario.Trim(), correo.Trim());
        }

        public void EnviarCodigoLogin(int usuarioAuthId, string mailProfile, int minutosExpira, bool debug)
        {
            if (usuarioAuthId <= 0)
                throw new ArgumentException("Usuario inválido.");

            _dal.EnviarCodigoLogin(usuarioAuthId, mailProfile, minutosExpira, debug);
        }

        public bool VerificarCodigoLogin(int usuarioAuthId, string codigo, int maxIntentos)
        {
            if (usuarioAuthId <= 0)
                throw new ArgumentException("Usuario inválido.");

            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código requerido.");

            return _dal.VerificarCodigoLogin(usuarioAuthId, codigo.Trim(), maxIntentos);
        }

        // ================= CAMBIAR PASSWORD PLAIN =================
        public void CambiarPasswordPlain(int usuarioId, string nuevaPassword, string editor)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("Usuario inválido.");

            if (string.IsNullOrWhiteSpace(nuevaPassword))
                throw new ArgumentException("La contraseña no puede estar vacía.");

            _dal.CambiarPasswordPlain(usuarioId, nuevaPassword, editor);
        }
    }
}