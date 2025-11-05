using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DPropiedad
    {
        private readonly string _cn;
        public DPropiedad(string connectionString) { _cn = connectionString; }

        public int Insertar(int unidadId, int propietarioId, decimal porcentaje, bool esTitularPrincipal, string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_insertar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@UnidadId", unidadId);
                cmd.Parameters.AddWithValue("@PropietarioId", propietarioId);
                cmd.Parameters.AddWithValue("@Porcentaje", porcentaje);
                cmd.Parameters.AddWithValue("@EsTitularPrincipal", esTitularPrincipal);
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);

                cn.Open();
                var o = cmd.ExecuteScalar();
                return Convert.ToInt32(o);
            }
        }

        public void Actualizar(int id, int unidadId, int propietarioId, decimal porcentaje, bool esTitularPrincipal, string usuario, byte[] rowVersion = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_actualizar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UnidadId", unidadId);
                cmd.Parameters.AddWithValue("@PropietarioId", propietarioId);
                cmd.Parameters.AddWithValue("@Porcentaje", porcentaje);
                cmd.Parameters.AddWithValue("@EsTitularPrincipal", esTitularPrincipal);
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
            using (var cmd = new SqlCommand("dbo.sp_propiedad_desactivar", cn) { CommandType = CommandType.StoredProcedure })
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
}
