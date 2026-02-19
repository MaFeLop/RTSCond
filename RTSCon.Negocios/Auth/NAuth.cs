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

            // Coincide con: U.ID_usr AS Id
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



        // ================= RECUPERACIÓN =================
        public int EnviarCodigoRecuperacion(string usuario, string correo)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.");

            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("Correo requerido.");

            return _dal.EnviarCodigoRecuperacion(usuario.Trim(), correo.Trim());
        }

        public bool ValidarCodigoRecuperacion(int usuarioId, string codigo)
        {
            if (usuarioId <= 0 || string.IsNullOrWhiteSpace(codigo))
                return false;

            return _dal.ValidarCodigoRecuperacion(usuarioId, codigo);
        }

        public void CambiarPasswordConCodigo(int usuarioId, string codigo, string nuevaPassword)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("Usuario inválido.");

            if (string.IsNullOrWhiteSpace(nuevaPassword))
                throw new ArgumentException("Nueva contraseña requerida.");

            _dal.CambiarPasswordConCodigo(usuarioId, codigo, nuevaPassword);
        }

        private string NormalizarRol(string rol)
        {
            rol = (rol ?? "").Trim();

            if (rol.Equals("SA", StringComparison.OrdinalIgnoreCase))
                return "SA";

            // Ajusta estos nombres para que coincidan con tbl_Rol.nombre
            if (rol.Equals("Propietario", StringComparison.OrdinalIgnoreCase))
                return "Propietario";

            if (rol.Equals("Secretario", StringComparison.OrdinalIgnoreCase))
                return "Secretario";

            if (rol.Equals("Inquilino", StringComparison.OrdinalIgnoreCase))
                return "Inquilino";

            // si te llega "Super Administrador" desde UI (por error)
            if (rol.IndexOf("Super", StringComparison.OrdinalIgnoreCase) >= 0)
                return "SA";

            throw new InvalidOperationException("Rol inválido.");
        }

    }
}
