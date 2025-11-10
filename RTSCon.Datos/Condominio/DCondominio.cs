using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DCondominio
    {
        private readonly string _cn;
        public DCondominio(string connectionString) { _cn = connectionString; }

        public int Insertar(string nombre, int? reglamentoDocumentoId, string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_crear", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);

                var pDoc = cmd.Parameters.Add("@ReglamentoDocumentoId", SqlDbType.Int);
                pDoc.Value = (object)reglamentoDocumentoId ?? DBNull.Value;

                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);

                cn.Open();
                var o = cmd.ExecuteScalar(); // SELECT SCOPE_IDENTITY()
                return Convert.ToInt32(o);
            }
        }
        public void Actualizar(int id, string nombre, int? reglamentoDocumentoId, string usuario, byte[] rowVersion = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_actualizar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                var pDoc = cmd.Parameters.Add("@ReglamentoDocumentoId", SqlDbType.Int);
                pDoc.Value = (object)reglamentoDocumentoId ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);

                var pRv = cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp);
                pRv.Value = (object)rowVersion ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Desactivar(int id, string usuario, byte[] rowVersion = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_desactivar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);
                var pRv = cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp);
                pRv.Value = (object)rowVersion ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_condominio_listar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@SoloActivos", soloActivos);
                da.SelectCommand.Parameters.AddWithValue("@Page", page);
                da.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                var dt = new DataTable();
                da.Fill(dt);

                totalRows = (dt.Rows.Count > 0 && dt.Columns.Contains("TotalRows"))
                            ? Convert.ToInt32(dt.Rows[0]["TotalRows"])
                            : 0;

                return dt;
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
