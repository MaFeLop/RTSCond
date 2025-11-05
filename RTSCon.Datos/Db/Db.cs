using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    internal static class Db
    {
        public static SqlConnection NewConnection(string cn) => new SqlConnection(cn);

        public static SqlParameter P(string name, object value, SqlDbType type, int size = 0)
        {
            var p = new SqlParameter(name, type);
            if (size > 0) p.Size = size;
            p.Value = value ?? DBNull.Value;
            return p;
        }

        public static object DbNullIfNull(object value) => value ?? DBNull.Value;
    }

    internal abstract class BaseDal
    {
        protected readonly string _cn;
        protected BaseDal(string cn) => _cn = cn;

        protected int ExecNonQuery(string sp, Action<SqlParameterCollection> bindParams,
                                   SqlTransaction tx = null)
        {
            using (var cmd = new SqlCommand(sp, tx?.Connection ?? Db.NewConnection(_cn)))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (tx == null) cmd.Connection.Open(); else cmd.Transaction = tx;
                bindParams?.Invoke(cmd.Parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        protected T ExecScalar<T>(string sp, Action<SqlParameterCollection> bindParams,
                                  SqlTransaction tx = null)
        {
            using (var cmd = new SqlCommand(sp, tx?.Connection ?? Db.NewConnection(_cn)))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (tx == null) cmd.Connection.Open(); else cmd.Transaction = tx;
                bindParams?.Invoke(cmd.Parameters);
                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return default(T);
                return (T)Convert.ChangeType(o, typeof(T));
            }
        }

        protected DataTable ExecTable(string sp, Action<SqlParameterCollection> bindParams)
        {
            using (var cn = Db.NewConnection(_cn))
            using (var da = new SqlDataAdapter(sp, cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                bindParams?.Invoke(da.SelectCommand.Parameters);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
