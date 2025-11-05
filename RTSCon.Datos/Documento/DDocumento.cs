using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DDocumento
    {
        private readonly string _cn;
        public DDocumento(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            _cn = connectionString;
        }

        // === MÉTODO QUE ESPERA NDocumento.InsertarProvisional(...) ===
        public int Insertar(string nombreArchivo, string tipoContenido, long tamanoBytes, string ubicacion, int version, string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_documento_insertar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@NombreArchivo", (object)nombreArchivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TipoContenido", (object)tipoContenido ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TamanoBytes", tamanoBytes);
                cmd.Parameters.AddWithValue("@Ubicacion", (object)ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Version", version);
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);

                cn.Open();
                var idObj = cmd.ExecuteScalar();
                return Convert.ToInt32(idObj);
            }
        }

        // === MÉTODO QUE ESPERA NDocumento.FinalizarUbicacion(...) ===
        public void ActualizarUbicacion(int id, string ubicacion, string usuario = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_documento_actualizar_ubicacion", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Ubicacion", (object)ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // (Opcionales, por si ya los tenías)
        public DataRow ObtenerPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_documento_obtener", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Id", id);
                var dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public void Desactivar(int id, byte[] rowVersion = null, string usuario = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_documento_desactivar", cn) { CommandType = CommandType.StoredProcedure })
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
