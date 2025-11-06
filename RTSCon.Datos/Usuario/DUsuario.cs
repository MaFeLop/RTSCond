using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Datos.Usuario
{
    public class DUsuario
    {
        private readonly string _cn;
        public DUsuario(string connectionString) { _cn = connectionString; }

        public DataTable Buscar(string buscar, bool soloActivos, int top = 20)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_usuario_buscar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@SoloActivos", soloActivos);
                da.SelectCommand.Parameters.AddWithValue("@Top", top);
                var dt = new DataTable(); da.Fill(dt); return dt;
            }
        }
    }

}
