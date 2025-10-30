using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos.Documento
{
    public sealed class DDocumento
    {
        private readonly string _cn;
        public DDocumento(string cn) => _cn = cn;

        public int Subir(string nombreArchivo, string tipoContenido, long tamanoBytes, string ubicacion, string usuario = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_documento_subir", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@NombreArchivo", nombreArchivo);
                cmd.Parameters.AddWithValue("@TipoContenido", tipoContenido ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TamanoBytes", tamanoBytes);
                cmd.Parameters.AddWithValue("@Ubicacion", ubicacion);
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);

                var pOut = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pOut);

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pOut.Value;
            }
        }

        public void Actualizar(int id, string nombreArchivo = null, string tipoContenido = null, long? tamanoBytes = null,
                               string ubicacion = null, byte[] rowVersion = null, string usuario = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_documento_actualizar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@NombreArchivo", (object)nombreArchivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TipoContenido", (object)tipoContenido ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TamanoBytes", (object)tamanoBytes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubicacion", (object)ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = (object)rowVersion ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    public sealed class DCondominioDocs
    {
        private readonly string _cn;
        public DCondominioDocs(string cn) => _cn = cn;

        public void AsignarReglamento(int condominioId, int documentoId, string usuario = null)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_reglamento_set", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@CondominioId", condominioId);
                cmd.Parameters.AddWithValue("@DocumentoId", documentoId);
                cmd.Parameters.AddWithValue("@Usuario", (object)usuario ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

}
