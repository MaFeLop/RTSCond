using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public static class DbUtil
    {
        public static SqlParameter AddIn<T>(SqlCommand cmd, string name, SqlDbType type, T value)
        {
            var p = cmd.Parameters.Add(name, type);
            p.Value = (object)value ?? DBNull.Value;
            return p;
        }

        public static SqlParameter AddOut(SqlCommand cmd, string name, SqlDbType type, int size = 0)
        {
            var p = cmd.Parameters.Add(name, type, size);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        public static object DbValue(DateTime? value) { return (object)value ?? DBNull.Value; }
        public static object DbValue(string value) { return (object)value ?? DBNull.Value; }
        public static object DbValue(byte[] value) { return (object)value ?? DBNull.Value; }
    }
}
