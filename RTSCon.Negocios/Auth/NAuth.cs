using System;
using System.Data;
using BCrypt.Net;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NAuth
    {
        private readonly DAuth _dal;

        public NAuth(DAuth dal)
        {
            _dal = dal;
        }

        public int CrearUsuario(string usuario, string correo, string password, int idRol)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuario requerido");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Contraseña requerida");

            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            return _dal.CrearUsuario(usuario.Trim(), correo?.Trim(), hash, idRol);
        }

        public void Login(string usuario, string password)
        {
            DataRow row = _dal.ObtenerPorUsuario(usuario);

            if (row == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            string hash = Convert.ToString(row["hash_bcrypt"]);

            if (!BCrypt.Net.BCrypt.Verify(password, hash))
                throw new InvalidOperationException("Contraseña incorrecta.");

            int id = Convert.ToInt32(row["ID_usr"]);
            string rol = Convert.ToString(row["ID_rol"]);

            UserContext.Set(id, usuario, rol);
            _dal.MarcarLogin(id);
        }

        public bool ValidarPassword(int idUsuario, string password)
        {
            DataRow row = _dal.ObtenerPorUsuario(UserContext.Usuario);
            if (row == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, Convert.ToString(row["hash_bcrypt"]));
        }
    }
}
