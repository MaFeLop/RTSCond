using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DCondominio
    {
        private readonly string _cn;
        public DCondominio(string connectionString) { _cn = connectionString; }

        public int Insertar(CondominioCrearRequest req)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_crear", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Nombre", req.Nombre);

                var pDoc = cmd.Parameters.Add("@ReglamentoDocumentoId", SqlDbType.Int);
                pDoc.Value = (object)req.ReglamentoDocumentoId ?? DBNull.Value;

                cmd.Parameters.AddWithValue("@Usuario", (object)req.Usuario ?? DBNull.Value);

                cn.Open();
                var result = cmd.ExecuteScalar(); // espera el SELECT SCOPE_IDENTITY()
                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("El SP no devolvió el Id del condominio.");

                return Convert.ToInt32(result);
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

    }

    // Puedes poner este DTO aquí o en la capa de Negocio (igual firma)
    public class CondominioCrearRequest
    {
        public string Nombre { get; set; }
        public int? ReglamentoDocumentoId { get; set; }
        public string Usuario { get; set; }
    }


}
