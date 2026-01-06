using System;
using System.Data;
using System.Security.Cryptography;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public static class UserContext
    {
        public static int UsuarioAuthId { get; private set; }
        public static string Usuario { get; private set; }
        public static string Rol { get; private set; }
        public static DateTime InicioSesionUtc { get; private set; }
        public static DateTime UltimaActividadUtc { get; private set; }   // 👈 NUEVO

        public static void Set(int id, string usuario, string rol)
        {
            UsuarioAuthId = id;
            Usuario = usuario;
            Rol = rol;
            InicioSesionUtc = DateTime.UtcNow;
            UltimaActividadUtc = InicioSesionUtc;                         // 👈
        }

        public static void Clear()
        {
            UsuarioAuthId = 0;
            Usuario = null;
            Rol = null;
            InicioSesionUtc = DateTime.MinValue;
            UltimaActividadUtc = DateTime.MinValue;
        }

        // registrar actividad (ratón/teclado)
        public static void Touch()
        {
            if (UsuarioAuthId != 0)
                UltimaActividadUtc = DateTime.UtcNow;
        }

        // ✅ ESTA ES LA PROPIEDAD QUE FALTABA
        public static bool IsLoggedIn => UsuarioAuthId != 0;

        public static bool EsSA =>
            string.Equals(Rol, "SA", StringComparison.OrdinalIgnoreCase);

        public static bool EsPropietarioActual =>
            string.Equals(Rol, "Propietario", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Rol, "Admin", StringComparison.OrdinalIgnoreCase);
    }


    public class NAuth
    {
        private readonly DAuth _dal;
        public NAuth(DAuth dal) { _dal = dal; }

        // PBKDF2 helpers (C# 7.3 compatible)
        public static void Derive(string password, int iter, out byte[] hash, out byte[] salt)
        {
            salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iter))
            {
                hash = pbkdf2.GetBytes(64);
            }
        }

        public static byte[] DeriveWithSalt(string password, byte[] salt, int iter)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iter))
            {
                return pbkdf2.GetBytes(64);
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        public int CrearUsuario(string usuario, string correo, string rol, string password, string creador, int iter = 150000)
        {
            if (string.IsNullOrWhiteSpace(usuario)) throw new ArgumentException("Usuario requerido.");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Contraseña requerida.");
            if (rol != "SA" && rol != "Admin" && rol != "Inquilino") throw new ArgumentException("Rol inválido.");

            byte[] hash, salt;
            Derive(password, iter, out hash, out salt);
            return _dal.CrearUsuario(usuario.Trim(), correo?.Trim(), rol, hash, salt, iter, creador);
        }

        /// <summary>
        /// Login solo por usuario + password (2FA desactivado).
        /// Devuelve el Id de UsuarioAuth si la contraseña es correcta.
        /// </summary>
        public int Login_Password(string usuario, string password, string mailProfile, int minutosCodigo, bool debug = false)
        {
            var row = _dal.ObtenerPorUsuario(usuario);
            if (row == null)
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            var id = Convert.ToInt32(row["Id"]);
            var iter = Convert.ToInt32(row["Iteraciones"]);
            var salt = (byte[])row["Salt"];
            var hash = (byte[])row["PasswordHash"];

            var prueba = DeriveWithSalt(password, salt, iter);
            if (!SlowEquals(hash, prueba))
                throw new InvalidOperationException("Usuario o contraseña inválidos.");

            // 2FA desactivado: ya no se envía código por correo.
            // _dal.EnviarCodigo(id, mailProfile, minutosCodigo, debug);

            return id;
        }

        /// <summary>
        /// Marca login y fija el UserContext (se mantiene el nombre por compatibilidad).
        /// </summary>
        public void Login_CodigoYSesion(int usuarioAuthId)
        {
            var row = _dal.ObtenerPorId(usuarioAuthId);
            var usuario = Convert.ToString(row["Usuario"]);
            var rol = Convert.ToString(row["Rol"]);
            UserContext.Set(usuarioAuthId, usuario, rol);
            _dal.MarcarLogin(usuarioAuthId);
        }

        // --- A partir de aquí todo queda igual, se usa para recuperación de contraseña y otras funciones ---

        public void VerificarCodigo(int usuarioAuthId, string codigo)
        {
            _dal.VerificarCodigo(usuarioAuthId, codigo);
        }

        public void CambiarPasswordConCodigo(int usuarioAuthId, string codigo, string nueva, string editor, int iter = 150000)
        {
            _dal.VerificarCodigo(usuarioAuthId, codigo);

            byte[] hash, salt;
            Derive(nueva, iter, out hash, out salt);
            _dal.CambiarPassword(usuarioAuthId, hash, salt, iter, editor);
        }

        public void CambiarPasswordConClave(int usuarioAuthId, string actual, string nueva, string editor)
        {
            var row = _dal.ObtenerPorId(usuarioAuthId);
            if (row == null) throw new InvalidOperationException("Usuario no encontrado.");

            var iter = Convert.ToInt32(row["Iteraciones"]);
            var salt = (byte[])row["Salt"];
            var hash = (byte[])row["PasswordHash"];

            var prueba = DeriveWithSalt(actual, salt, iter);
            if (!SlowEquals(hash, prueba)) throw new InvalidOperationException("Contraseña actual inválida.");

            byte[] newHash, newSalt;
            Derive(nueva, iter, out newHash, out newSalt);
            _dal.CambiarPassword(usuarioAuthId, newHash, newSalt, iter, editor);
        }

        public bool Login_Codigo(int usuarioAuthId, string codigo)
        {
            if (usuarioAuthId <= 0) return false;
            if (string.IsNullOrWhiteSpace(codigo)) return false;

            try
            {
                _dal.VerificarCodigo(usuarioAuthId, codigo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int EnviarCodigoRecuperacion(string usuario, string correo, string mailProfile, int minutosCodigo, bool debug = false)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido.", nameof(usuario));
            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("Correo requerido.", nameof(correo));

            var row = _dal.ObtenerPorUsuario(usuario.Trim());
            if (row == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            if (row["Correo"] == DBNull.Value)
                throw new InvalidOperationException("El usuario no tiene un correo vinculado.");

            var correoDb = Convert.ToString(row["Correo"]).Trim();

            if (!string.Equals(correoDb, correo.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("El correo no coincide con el usuario.");

            var id = Convert.ToInt32(row["Id"]);
            _dal.EnviarCodigo(id, mailProfile, minutosCodigo, debug);
            return id;
        }

        public void ReenviarCodigo(int usuarioAuthId, string mailProfile, int minutosCodigo, bool debug = false)
        {
            if (usuarioAuthId <= 0) throw new ArgumentException("UsuarioAuthId inválido.", nameof(usuarioAuthId));
            _dal.EnviarCodigo(usuarioAuthId, mailProfile, minutosCodigo, debug);
        }

        public string CorreoEnmascarado(int usuarioAuthId)
        {
            var row = _dal.ObtenerPorId(usuarioAuthId);
            if (row == null || row["Correo"] == DBNull.Value) return "(correo no disponible)";
            var email = Convert.ToString(row["Correo"]);
            return MaskEmail(email);
        }

        private static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) return email;
            var parts = email.Split('@');
            var user = parts[0];
            var dom = parts[1];
            var head = user.Length <= 2 ? user.Substring(0, 1) : user.Substring(0, 2);
            return head + new string('*', Math.Max(1, user.Length - head.Length)) + "@" + dom;
        }

        public DataTable ListarPropietarios(string buscar, bool soloActivos, int page, int pageSize, out int totalRows)
        {
            return _dal.ListarPropietarios(buscar, soloActivos, page, pageSize, out totalRows);
        }

        public bool ValidarPassword(int usuarioAuthId, string password)
        {
            if (usuarioAuthId <= 0 || string.IsNullOrWhiteSpace(password)) return false;
            var row = _dal.ObtenerPorId(usuarioAuthId);
            if (row == null) return false;

            var iter = Convert.ToInt32(row["Iteraciones"]);
            var salt = (byte[])row["Salt"];
            var hash = (byte[])row["PasswordHash"];
            var prueba = DeriveWithSalt(password, salt, iter);
            return SlowEquals(hash, prueba);
        }
    }
}
