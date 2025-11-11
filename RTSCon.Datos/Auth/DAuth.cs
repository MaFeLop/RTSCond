using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DAuth
    {
        private readonly string _cn;
        public DAuth(string connectionString) { _cn = connectionString; }

        public int CrearUsuario(string usuario, string correo, string rol,
                                byte[] hash, byte[] salt, int iteraciones, string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuarioauth_crear", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Correo", (object)correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Rol", rol);
                var p1 = cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, 64); p1.Value = hash;
                var p2 = cmd.Parameters.Add("@Salt", SqlDbType.VarBinary, 16); p2.Value = salt;
                cmd.Parameters.AddWithValue("@Iteraciones", iteraciones);
                cmd.Parameters.AddWithValue("@UsuarioCreador", (object)creador ?? DBNull.Value);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public DataRow ObtenerPorUsuario(string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_usuarioauth_por_usuario", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Usuario", usuario);
                var dt = new DataTable(); da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public DataRow ObtenerPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_usuarioauth_por_id", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Id", id);
                var dt = new DataTable(); da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public void MarcarLogin(int usuarioAuthId)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuarioauth_marcar_login", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@UsuarioAuthId", usuarioAuthId);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        public void EnviarCodigo(int usuarioAuthId, string mailProfile, int minutosExpira, bool debug)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_logincode_enviar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@UsuarioAuthId", usuarioAuthId);
                cmd.Parameters.AddWithValue("@MailProfile", (object)mailProfile ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MinutosExpira", minutosExpira);
                cmd.Parameters.AddWithValue("@Debug", debug);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        public void VerificarCodigo(int usuarioAuthId, string codigo)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_logincode_verificar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@UsuarioAuthId", usuarioAuthId);
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        public void CambiarPassword(int usuarioAuthId, byte[] newHash, byte[] newSalt, int iteraciones, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuarioauth_cambiar_password", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@UsuarioAuthId", usuarioAuthId);
                var p1 = cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, 64); p1.Value = newHash;
                var p2 = cmd.Parameters.Add("@Salt", SqlDbType.VarBinary, 16); p2.Value = newSalt;
                cmd.Parameters.AddWithValue("@Iteraciones", iteraciones);
                cmd.Parameters.AddWithValue("@UsuarioEditor", (object)editor ?? DBNull.Value);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }
    }
}
