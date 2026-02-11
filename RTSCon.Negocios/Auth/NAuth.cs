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

        public static bool IsLoggedIn => UsuarioAuthId > 0;

        public static void Set(int id, string usuario, string rol)
        {
            UsuarioAuthId = id;
            Usuario = usuario;
            Rol = rol;
        }

        public static void Clear()
        {
            UsuarioAuthId = 0;
            Usuario = null;
            Rol = null;
        }
    }

    public class NAuth
    {
        private readonly DAuth _dal;

        public NAuth(DAuth dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

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
    }
}
