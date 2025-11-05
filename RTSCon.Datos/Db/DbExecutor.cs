using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Datos
{
    public static class DbExecutor
    {
        public static int ExecuteNonQuery(string spName, params SqlParameter[] parameters)
        {
            using (var cn = SqlConnectionFactory.Create())
            using (var cmd = new SqlCommand(spName, cn) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters?.Length > 0) cmd.Parameters.AddRange(parameters);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataSet ExecuteDataSet(string spName, params SqlParameter[] parameters)
        {
            using (var cn = SqlConnectionFactory.Create())
            using (var cmd = new SqlCommand(spName, cn) { CommandType = CommandType.StoredProcedure })
            using (var da = new SqlDataAdapter(cmd))
            {
                if (parameters?.Length > 0) cmd.Parameters.AddRange(parameters);
                var ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }
    }
}
