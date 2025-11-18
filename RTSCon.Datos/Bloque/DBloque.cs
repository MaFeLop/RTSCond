// RTSCon.Datos\DBloque.cs
using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DBloque
    {
        private readonly string _cn;

        public DBloque(string connectionString)
        {
            _cn = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // Listar bloques por condominio (sin filtro de texto)
        public DataTable ListarPorCondominio(int condominioId)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_bloque_listar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@CondominioId", SqlDbType.Int).Value = condominioId;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt;
            }
        }

        // Buscar con texto y soloActivos (Top N)
        public DataTable Buscar(int? condominioId, string buscar, bool soloActivos, int top = 20)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_bloque_buscar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@CondominioId", SqlDbType.Int).Value =
                    (object)condominioId ?? DBNull.Value;
                da.SelectCommand.Parameters.Add("@Buscar", SqlDbType.NVarChar, 80).Value =
                    (object)buscar ?? DBNull.Value;
                da.SelectCommand.Parameters.Add("@SoloActivos", SqlDbType.Bit).Value = soloActivos;
                da.SelectCommand.Parameters.Add("@Top", SqlDbType.Int).Value = top;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt;
            }
        }

        // Insertar un bloque
        public int Insertar(int condominioId, string identificador, int numeroPisos, int unidadesPorPiso, string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_bloque_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@CondominioId", SqlDbType.Int).Value = condominioId;
                cmd.Parameters.Add("@Identificador", SqlDbType.NVarChar, 50).Value =
                    (object)identificador ?? DBNull.Value;
                cmd.Parameters.Add("@NumeroPisos", SqlDbType.Int).Value = numeroPisos;
                cmd.Parameters.Add("@UnidadesPorPiso", SqlDbType.Int).Value = unidadesPorPiso;
                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                    (object)creador ?? DBNull.Value;

                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int);
                pId.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        // Actualizar con RowVersion
        public void Actualizar(
            int id,
            string identificador,
            int numeroPisos,
            int unidadesPorPiso,
            byte[] rowVersion,
            string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_bloque_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Identificador", SqlDbType.NVarChar, 50).Value =
                    (object)identificador ?? DBNull.Value;
                cmd.Parameters.Add("@NumeroPisos", SqlDbType.Int).Value = numeroPisos;
                cmd.Parameters.Add("@UnidadesPorPiso", SqlDbType.Int).Value = unidadesPorPiso;

                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                    (object)editor ?? DBNull.Value;
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value =
                    (object)rowVersion ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Desactivar (soft delete) con RowVersion
        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_bloque_eliminar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                    (object)editor ?? DBNull.Value;
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value =
                    (object)rowVersion ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // PorId (DataRow) – igual estilo que DCondominio.PorId
        public DataRow PorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_bloque_obtener", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // BuscarPorId (DataTable) – igual patrón que BuscarPorId en DCondominio
        public DataTable BuscarPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_bloque_obtener", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
