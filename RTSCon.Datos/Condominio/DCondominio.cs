using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DCondominio
    {
        private readonly string _cn;
        public DCondominio(string connectionString) { _cn = connectionString; }

        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows)
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
                var pTotal = cmd.Parameters.Add("@totalRows", SqlDbType.Int);
                pTotal.Direction = ParameterDirection.Output;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                totalRows = (int)pTotal.Value;
                return dt;
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

        public int Insertar(string nombre, int? reglaDocId, string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_insertar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@ReglamentoDocumentoId", (object)reglaDocId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Creador", creador);
                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int);
                pId.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        public void Actualizar(int id, string nombre, int? reglaDocId, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@ReglamentoDocumentoId", (object)reglaDocId ?? DBNull.Value);
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp, 8).Value = rowVersion;
                cmd.Parameters.AddWithValue("@Editor", editor);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_desactivar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp, 8).Value = rowVersion;
                cmd.Parameters.AddWithValue("@Editor", editor);
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
            

    }


    // Puedes poner este DTO aquí o en la capa de Negocio (igual firma)
    public class CondominioCrearRequest
    {
        public string Nombre { get; set; }
        public int? ReglamentoDocumentoId { get; set; }
        public string Usuario { get; set; }
    }


}
