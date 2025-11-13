using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DPropiedad
    {
        private readonly string _cn;
        public DPropiedad(string cn) { _cn = cn; }

        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows, int? ownerId = null)
        {
            totalRows = 0;
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_listar", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);
                cmd.Parameters.AddWithValue("@Page", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                var pOwner = cmd.Parameters.Add("@OwnerId", SqlDbType.Int);
                pOwner.Value = (object)ownerId ?? DBNull.Value;

                var pTotal = cmd.Parameters.Add("@TotalRows", SqlDbType.Int);
                pTotal.Direction = ParameterDirection.Output;

                var dt = new DataTable();
                da.Fill(dt);
                totalRows = (pTotal.Value == DBNull.Value) ? 0 : Convert.ToInt32(pTotal.Value);
                return dt;
            }
        }

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

        public int Insertar(int propietarioId, int unidadId, DateTime? fechaInicio, DateTime? fechaFin,
                            decimal porcentaje, bool esTitularPrincipal, string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_insertar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PropietarioId", propietarioId);
                cmd.Parameters.AddWithValue("@UnidadId", unidadId);
                cmd.Parameters.AddWithValue("@FechaInicio", (object)fechaInicio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaFin", (object)fechaFin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Porcentaje", porcentaje);
                cmd.Parameters.AddWithValue("@EsTitularPrincipal", esTitularPrincipal);
                cmd.Parameters.AddWithValue("@Creador", (object)creador ?? DBNull.Value);

                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int);
                pId.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        public void Actualizar(int id, int propietarioId, int unidadId, DateTime? fechaInicio, DateTime? fechaFin,
                               decimal porcentaje, bool esTitularPrincipal, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@PropietarioId", propietarioId);
                cmd.Parameters.AddWithValue("@UnidadId", unidadId);
                cmd.Parameters.AddWithValue("@FechaInicio", (object)fechaInicio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaFin", (object)fechaFin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Porcentaje", porcentaje);
                cmd.Parameters.AddWithValue("@EsTitularPrincipal", esTitularPrincipal);
                cmd.Parameters.AddWithValue("@RowVersion", (object)rowVersion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Editor", (object)editor ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_desactivar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@RowVersion", (object)rowVersion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Editor", (object)editor ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
