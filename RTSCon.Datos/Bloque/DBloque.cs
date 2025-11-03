// DBloque.cs
using System;
using System.Data;
using System.Data.SqlClient;
using RTSCon.Entidad;

namespace RTSCon.Datos
{
    public sealed class DBloque
    {
        private readonly string _cn;
        public DBloque(string connectionString) { _cn = connectionString; }

        private static SqlParameter P(string name, SqlDbType type, object value, int size, ParameterDirection dir)
        {
            var p = new SqlParameter(name, type);
            if (size > 0) p.Size = size;
            p.Direction = dir;
            p.Value = (value ?? DBNull.Value);
            return p;
        }

        public DataTable Listar(int condominioId)
        {
            var dt = new DataTable();
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_bloque_listar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(P("@CondominioId", SqlDbType.Int, condominioId, 0, ParameterDirection.Input));
                cn.Open();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
            }
            return dt;
        }

        public int Crear(EBloque b, string usuario, out int nuevoId)
        {
            nuevoId = 0;
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_bloque_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(P("@CondominioId", SqlDbType.Int, b.CondominioId, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Identificador", SqlDbType.NVarChar, b.Identificador, 50, ParameterDirection.Input));
                cmd.Parameters.Add(P("@NumeroPisos", SqlDbType.Int, b.NumeroPisos, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@UnidadesPorPiso", SqlDbType.Int, b.UnidadesPorPiso, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Usuario", SqlDbType.NVarChar, usuario, 100, ParameterDirection.Input));
                var outId = P("@NuevoId", SqlDbType.Int, null, 0, ParameterDirection.Output);
                cmd.Parameters.Add(outId);
                cn.Open();
                cmd.ExecuteNonQuery();
                nuevoId = (outId.Value == DBNull.Value) ? 0 : Convert.ToInt32(outId.Value);
            }
            return nuevoId;
        }

        public void Actualizar(EBloque b, string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_bloque_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(P("@Id", SqlDbType.Int, b.Id, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Identificador", SqlDbType.NVarChar, b.Identificador, 50, ParameterDirection.Input));
                cmd.Parameters.Add(P("@NumeroPisos", SqlDbType.Int, b.NumeroPisos, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@UnidadesPorPiso", SqlDbType.Int, b.UnidadesPorPiso, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Usuario", SqlDbType.NVarChar, usuario, 100, ParameterDirection.Input));
                cmd.Parameters.Add(P("@RowVersion", SqlDbType.Timestamp, b.RowVersion, 8, ParameterDirection.Input));
                cn.Open();
                cmd.ExecuteNonQuery(); // Levanta error del SP si hay conflicto de concurrencia
            }
        }

        public void Eliminar(int id, byte[] rowVersion, string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_bloque_eliminar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(P("@Id", SqlDbType.Int, id, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Usuario", SqlDbType.NVarChar, usuario, 100, ParameterDirection.Input));
                cmd.Parameters.Add(P("@RowVersion", SqlDbType.Timestamp, rowVersion, 8, ParameterDirection.Input));
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
