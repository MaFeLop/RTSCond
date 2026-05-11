using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DDashboard
    {
        private readonly string _connectionString;

        public DDashboard(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión es requerida.");

            _connectionString = connectionString;
        }

        public DataRow ObtenerResumen()
        {
            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_dashboard_resumen", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = new DataTable();
                da.Fill(tabla);

                if (tabla.Rows.Count == 0)
                    return null;

                return tabla.Rows[0];
            }
        }

        public DataTable ObtenerUltimosEventos()
        {
            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_dashboard_ultimos_eventos", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = new DataTable();
                da.Fill(tabla);

                return tabla;
            }
        }
    }
}