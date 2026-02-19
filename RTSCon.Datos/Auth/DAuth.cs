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

        // ================= LOGIN =================
        public DataRow Login(string usuario, string password)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_login", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = usuario;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 400).Value = password;

                var dt = new DataTable();

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // ================= VALIDAR PASSWORD =================
        public bool ValidarPassword(int usuarioId, string password)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_validar_password", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id_usr", SqlDbType.Int).Value = usuarioId;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 400).Value = password;

                cn.Open();

                object result = cmd.ExecuteScalar();

                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        // ================= CREAR USUARIO =================
        public int CrearUsuario(string usuario, string correo, string password, int idRol)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@usuario", SqlDbType.NVarChar, 200).Value = usuario;
                cmd.Parameters.Add("@correo", SqlDbType.NVarChar, 512).Value = (object)correo ?? DBNull.Value;
                cmd.Parameters.Add("@pass_plain", SqlDbType.NVarChar, 400).Value = password;
                cmd.Parameters.Add("@id_rol", SqlDbType.Int).Value = idRol;

                cn.Open();

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }




        // ================= RECUPERACIÓN =================
        public int EnviarCodigoRecuperacion(string usuario, string correo)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_recuperacion_enviar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = usuario;
                cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 256).Value = correo;

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

                cmd.Parameters.Add("@id_usr", SqlDbType.Int).Value = usuarioId;
                cmd.Parameters.Add("@Codigo", SqlDbType.NVarChar, 20).Value = codigo;

                cn.Open();

                object result = cmd.ExecuteScalar();

                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        public void CambiarPasswordConCodigo(int usuarioId, string codigo, string nuevaPassword)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_recuperacion_cambiar_password", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id_usr", SqlDbType.Int).Value = usuarioId;
                cmd.Parameters.Add("@Codigo", SqlDbType.NVarChar, 20).Value = codigo;
                cmd.Parameters.Add("@NuevaPassword", SqlDbType.NVarChar, 200).Value = nuevaPassword;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ================= MARCAR LOGIN =================
        public void MarcarLogin(int usuarioId)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_marcar_login", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id_usr", SqlDbType.Int).Value = usuarioId;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int ObtenerIdRol(string nombreRol)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand(@"
        SELECT ID_rol
        FROM dbo.tbl_Rol
        WHERE nombre = @nombre
    ", cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombreRol;

                cn.Open();
                var result = cmd.ExecuteScalar();

                if (result == null)
                    return 0;

                return Convert.ToInt32(result);
            }
        }


        public int ObtenerIdRolPorNombre(string nombreRol)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 ID_rol
        FROM dbo.tbl_Rol
        WHERE UPPER(LTRIM(RTRIM(nombre))) = UPPER(LTRIM(RTRIM(@nombre)));
    ", cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 200).Value = nombreRol ?? "";

                cn.Open();
                var obj = cmd.ExecuteScalar();
                return (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt32(obj);
            }
        }

    }
}
