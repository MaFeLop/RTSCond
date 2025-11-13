using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DPropiedad
    {
        private readonly string _cn;
        public DPropiedad(string connectionString) { _cn = connectionString; }

        // LISTAR (paginado)
        public DataTable Listar(string buscar, bool soloActivas, int page, int pageSize, out int totalRows)
        {
            totalRows = 0;
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_listar", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoloActivas", soloActivas);
                cmd.Parameters.AddWithValue("@Page", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                var pTotal = cmd.Parameters.Add("@TotalRows", SqlDbType.Int); pTotal.Direction = ParameterDirection.Output;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                totalRows = (pTotal.Value == DBNull.Value) ? 0 : Convert.ToInt32(pTotal.Value);
                return dt;
            }
        }

        // POR ID
        public DataRow PorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_propiedad_por_id", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Id", id);
                var dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // INSERTAR
        // Ajusta nombres de parámetros a tu tabla dbo.Propiedad
        public int Insertar(
            string nombre,
            string tipo,
            string ubicacion,
            decimal? cuotaMantenimientoBase,
            int? condominioId,
            string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_insertar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Tipo", (object)tipo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubicacion", (object)ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CuotaMantenimientoBase", (object)cuotaMantenimientoBase ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CondominioId", (object)condominioId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Creador", (object)creador ?? DBNull.Value);
                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int); pId.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(pId.Value);
            }
        }

        // ACTUALIZAR (concurrency por RowVersion)
        public void Actualizar(
            int id,
            string nombre,
            string tipo,
            string ubicacion,
            decimal? cuotaMantenimientoBase,
            int? condominioId,
            byte[] rowVersion,
            string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Tipo", (object)tipo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubicacion", (object)ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CuotaMantenimientoBase", (object)cuotaMantenimientoBase ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CondominioId", (object)condominioId ?? DBNull.Value);
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = (object)rowVersion ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@Editor", (object)editor ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // DESACTIVAR (borrado lógico)
        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_desactivar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = (object)rowVersion ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@Editor", (object)editor ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // (Opcional) Notificación, espejo del patrón en DCondominio
        public void NotificarAccion(int id, string accion, string usuarioEditor, string mailProfile)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_notificar_accion", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Accion", accion);
                cmd.Parameters.AddWithValue("@UsuarioEditor", (object)usuarioEditor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MailProfile", (object)mailProfile ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
