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
                cmd.Parameters.Add("@correo", SqlDbType.NVarChar, 512).Value =
                    (object)correo ?? DBNull.Value;
                cmd.Parameters.Add("@pass_plain", SqlDbType.NVarChar, 400).Value = password;
                cmd.Parameters.Add("@id_rol", SqlDbType.Int).Value = idRol;

                cn.Open();

                return Convert.ToInt32(cmd.ExecuteScalar());
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

        // ================= ROLES =================
        public int ObtenerIdRol(string nombreRol)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 ID_rol
                FROM dbo.tbl_Rol
                WHERE UPPER(LTRIM(RTRIM(nombre))) = UPPER(LTRIM(RTRIM(@nombre)));
            ", cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombreRol ?? "";

                cn.Open();

                object result = cmd.ExecuteScalar();
                return (result == null || result == DBNull.Value)
                    ? 0
                    : Convert.ToInt32(result);
            }
        }

        // ================= RECUPERACIÓN / CÓDIGO =================
        public int ObtenerUsuarioAuthIdPorUsuarioYCorreo(string usuario, string correo)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 ID_usr
        FROM dbo.tbl_Usuario
        WHERE LTRIM(RTRIM(usuario)) = LTRIM(RTRIM(@usuario))
          AND LTRIM(RTRIM(correo)) = LTRIM(RTRIM(@correo))
          AND UPPER(LTRIM(RTRIM(estado))) = 'ACTIVO';
    ", cn))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@usuario", SqlDbType.VarChar, 60).Value = (usuario ?? "").Trim();
                cmd.Parameters.Add("@correo", SqlDbType.VarChar, 256).Value = (correo ?? "").Trim();

                cn.Open();

                object result = cmd.ExecuteScalar();
                return (result == null || result == DBNull.Value)
                    ? 0
                    : Convert.ToInt32(result);
            }
        }

        public void EnviarCodigoLogin(int usuarioAuthId, string mailProfile, int minutosExpira, bool debug)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_logincode_enviar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@UsuarioAuthId", SqlDbType.Int).Value = usuarioAuthId;
                cmd.Parameters.Add("@MailProfile", SqlDbType.NVarChar, 128).Value = mailProfile;
                cmd.Parameters.Add("@MinutosExpira", SqlDbType.Int).Value = minutosExpira;
                cmd.Parameters.Add("@Debug", SqlDbType.Bit).Value = debug;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool VerificarCodigoLogin(int usuarioAuthId, string codigo, int maxIntentos)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_logincode_verificar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@UsuarioAuthId", SqlDbType.Int).Value = usuarioAuthId;
                cmd.Parameters.Add("@Codigo", SqlDbType.NVarChar, 6).Value = codigo;
                cmd.Parameters.Add("@MaxIntentos", SqlDbType.Int).Value = maxIntentos;

                cn.Open();

                object result = cmd.ExecuteScalar();
                return result != null
                    && result != DBNull.Value
                    && Convert.ToInt32(result) == 1;
            }
        }

        // ================= CAMBIAR PASSWORD PLAIN =================
        public void CambiarPasswordPlain(int usuarioId, string nuevaPassword, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_usuario_cambiar_password_plain", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@UsuarioAuthId", SqlDbType.Int).Value = usuarioId;
                cmd.Parameters.Add("@NuevaPassword", SqlDbType.NVarChar, 400).Value = nuevaPassword;
                cmd.Parameters.Add("@UsuarioEditor", SqlDbType.NVarChar, 100).Value = editor;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}