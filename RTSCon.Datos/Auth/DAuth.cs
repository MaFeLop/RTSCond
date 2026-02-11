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
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlDataAdapter da = new SqlDataAdapter("dbo.sp_usuario_login", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Usuario", usuario);
                da.SelectCommand.Parameters.AddWithValue("@Password", password);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }


        public int CrearUsuario(string usuario, string correo, string passwordPlain, int idRol)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_usuario_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@correo", (object)correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@pass_plain", passwordPlain);
                cmd.Parameters.AddWithValue("@id_rol", idRol);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        public DataRow ObtenerPorUsuario(string usuario)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_usuarioauth_por_usuario", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario); // OJO: @Usuario (como tu SP)

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }


        public DataRow ObtenerPorId(int idUsuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("sp_usuarioauth_por_id", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }


        public void MarcarLogin(int idUsuario)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_usuario_marcar_login", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usr", idUsuario);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
