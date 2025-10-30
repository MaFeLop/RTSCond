using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos.Condominio
{
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
