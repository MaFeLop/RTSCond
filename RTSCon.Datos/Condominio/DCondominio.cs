using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DCondominio
    {
        private readonly string _cn;
        public DCondominio(string connectionString) { _cn = connectionString; }

        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, int? idPropietario, out int totalRows)
        {
            totalRows = 0;

            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_listar", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@buscar", (object)buscar ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@soloActivos", soloActivos);
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);

                cmd.Parameters.AddWithValue("@ID_propietario", (object)idPropietario ?? DBNull.Value);

                var pTotal = cmd.Parameters.Add("@totalRows", SqlDbType.Int);
                pTotal.Direction = ParameterDirection.Output;
                pTotal.Value = 0; // evita null

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);

                totalRows = (pTotal.Value == null || pTotal.Value == DBNull.Value)
                            ? 0
                            : Convert.ToInt32(pTotal.Value);

                return dt;
            }
        }

        public void Notificar(string to, string subject, string htmlBody, string mailProfile)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_notificar_accion", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Para", to ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Asunto", subject ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Cuerpo", htmlBody ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Perfil", mailProfile ?? (object)DBNull.Value);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }



        public DataRow PorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_por_id", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public int Insertar(
    string nombre, string direccion, string tipo, string adminResp,
    DateTime fechaConstitucion, decimal cuotaBase,
    string emailNotif, bool enviarNotifProp,
    int? reglamentoDocId, int? idPropietario, int? idSec1, int? idSec2, int? idSec3,
    string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_insertar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Direccion", direccion);
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@AdministradorResponsable", adminResp);
                cmd.Parameters.AddWithValue("@FechaConstitucion", fechaConstitucion);
                cmd.Parameters.AddWithValue("@CuotaMantenimientoBase", cuotaBase);
                cmd.Parameters.AddWithValue("@EmailNotificaciones", emailNotif);
                cmd.Parameters.AddWithValue("@EnviarNotifsAlPropietario", enviarNotifProp);
                cmd.Parameters.AddWithValue("@ReglamentoDocumentoId", (object)reglamentoDocId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_propietario", (object)idPropietario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_usr_secretario1", (object)idSec1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_usr_secretario2", (object)idSec2 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_usr_secretario3", (object)idSec3 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Creador", creador);

                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int);
                pId.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }


        // ⬇️ NUEVO: traer 1 condominio por Id
        public DataTable BuscarPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("sp_condominio_por_id", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt;
            }
        }

        // ⬇️ NUEVO: actualizar con RowVersion (concurrencia optimista)
        public void Actualizar(
            int id,
            string nombre, string direccion, string tipo, string administradorResponsable,
            DateTime fechaConstitucion, decimal cuotaMantenimientoBase,
            string emailNotificaciones, bool enviarNotifsAlPropietario,
            int? reglamentoDocumentoId, int? idPropietario, int? idSecretario1, int? idSecretario2, int? idSecretario3,
            byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("sp_condominio_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value = (object)nombre ?? DBNull.Value;
                cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 300).Value = (object)direccion ?? DBNull.Value;
                cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 30).Value = (object)tipo ?? DBNull.Value;
                cmd.Parameters.Add("@AdministradorResponsable", SqlDbType.NVarChar, 150).Value = (object)administradorResponsable ?? DBNull.Value;
                cmd.Parameters.Add("@FechaConstitucion", SqlDbType.Date).Value = fechaConstitucion;
                cmd.Parameters.Add("@CuotaMantenimientoBase", SqlDbType.Decimal).Value = cuotaMantenimientoBase;
                cmd.Parameters.Add("@EmailNotificaciones", SqlDbType.NVarChar, 150).Value = (object)emailNotificaciones ?? DBNull.Value;
                cmd.Parameters.Add("@EnviarNotifsAlPropietario", SqlDbType.Bit).Value = enviarNotifsAlPropietario;

                cmd.Parameters.Add("@ReglamentoDocumentoId", SqlDbType.Int).Value = (object)reglamentoDocumentoId ?? DBNull.Value;
                cmd.Parameters.Add("@ID_propietario", SqlDbType.Int).Value = (object)idPropietario ?? DBNull.Value;
                cmd.Parameters.Add("@ID_usr_secretario1", SqlDbType.Int).Value = (object)idSecretario1 ?? DBNull.Value;
                cmd.Parameters.Add("@ID_usr_secretario2", SqlDbType.Int).Value = (object)idSecretario2 ?? DBNull.Value;
                cmd.Parameters.Add("@ID_usr_secretario3", SqlDbType.Int).Value = (object)idSecretario3 ?? DBNull.Value;

                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = (object)rowVersion ?? DBNull.Value;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = (object)editor ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ⬇️ Si cambiaste Desactivar a usar RowVersion+Editor:
        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("sp_condominio_desactivar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = (object)rowVersion ?? DBNull.Value;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = (object)editor ?? DBNull.Value;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public DataRow ObtenerPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_condominio_obtener", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Id", id);
                var dt = new DataTable(); da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public DataTable Buscar(string buscar, bool soloActivos, int top = 20)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_condominio_buscar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@SoloActivos", soloActivos);
                da.SelectCommand.Parameters.AddWithValue("@Top", top);
                var dt = new DataTable(); da.Fill(dt); return dt;
            }
        }

        // RTSCon.Datos\DCondominio.cs  (añade)
        public void NotificarAccion(int id, string accion, string editor, string mailProfile)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_notificar_accion", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Accion", (object)accion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Editor", (object)editor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MailProfile", (object)mailProfile ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }


    // Puedes poner este DTO aquí o en la capa de Negocio (igual firma)
    public class CondominioCrearRequest
    {
        public string Nombre { get; set; }
        public int? ReglamentoDocumentoId { get; set; }
        public string Usuario { get; set; }
    }


}
