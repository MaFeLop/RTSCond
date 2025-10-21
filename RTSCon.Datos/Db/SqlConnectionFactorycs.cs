using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Datos.Db
{
    public static class SqlConnectionFactory
    {
        public static SqlConnection Create()
        {
            var cs = ConfigurationManager.ConnectionStrings["CondoDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException(
                    "No se encontró la conexion" +
                    "Por favor contacte a un administrador por este error.");

            return new SqlConnection(cs);
        }
    }
}
