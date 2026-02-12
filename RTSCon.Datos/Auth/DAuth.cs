using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DAuth
    {
        private readonly string _cn;

        public DAuth(string connectionString)
        {
            _cn = connectionString;
        }

        public DataRow Login(string usuario, string password)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_usuario_login", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Usuario", usuario);
                da.SelectCommand.Parameters.AddWithValue("@Password", password);

                var dt = new DataTable();
                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public bool ValidarPassword(int usuarioId, string password)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_validar_password", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", usuarioId);
                cmd.Parameters.AddWithValue("@Password", password);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        public int CrearUsuario(string usuario, string correo, string password, string rol)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Correo", (object)correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Rol", rol);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int EnviarCodigoRecuperacion(string usuario, string correo)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_recuperacion_enviar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Correo", correo);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool ValidarCodigoRecuperacion(int usuarioId, string codigo)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_recuperacion_validar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", usuarioId);
                cmd.Parameters.AddWithValue("@Codigo", codigo);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        public void CambiarPasswordConCodigo(int usuarioId, string codigo, string nuevaPassword)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_recuperacion_cambiar_password", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", usuarioId);
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                cmd.Parameters.AddWithValue("@NuevaPassword", nuevaPassword);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MarcarLogin(int usuarioId)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_marcar_login", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", usuarioId);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
