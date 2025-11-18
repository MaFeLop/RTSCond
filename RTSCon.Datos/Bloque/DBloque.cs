using RTSCon.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace RTSCon.Datos
{
    public class DBloque
    {
        public IEnumerable<EBloque> Listar(int condominioId)
        {
            var lista = new List<EBloque>();

            using (var conn = SqlConnectionFactory.CreateConnection()) // TODO: ajusta helper
            using (var cmd = new SqlCommand("sp_bloque_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CondominioId", condominioId);

                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(Mapear(dr));
                    }
                }
            }

            return lista;
        }

        public IEnumerable<EBloque> Buscar(int? condominioId, string buscar, bool soloActivos, int top = 20)
        {
            var lista = new List<EBloque>();

            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("sp_bloque_buscar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CondominioId", (object)condominioId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);
                cmd.Parameters.AddWithValue("@Top", top);

                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(Mapear(dr));
                    }
                }
            }

            return lista;
        }

        public EBloque ObtenerPorId(int id)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("sp_bloque_obtener", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        return Mapear(dr);
                }
            }

            return null;
        }

        public int Crear(EBloque bloque, string usuario)
        {
            if (bloque == null) throw new ArgumentNullException(nameof(bloque));

            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("sp_bloque_crear", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CondominioId", bloque.CondominioId);
                cmd.Parameters.AddWithValue("@Identificador", bloque.Identificador);
                cmd.Parameters.AddWithValue("@NumeroPisos", bloque.NumeroPisos);
                cmd.Parameters.AddWithValue("@UnidadesPorPiso", bloque.UnidadesPorPiso);
                cmd.Parameters.AddWithValue("@Usuario", usuario ?? (object)DBNull.Value);

                var pId = new SqlParameter("@NuevoId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pId);

                conn.Open();
                cmd.ExecuteNonQuery();

                return (int)pId.Value;
            }
        }

        public void Actualizar(EBloque bloque, string usuario)
        {
            if (bloque == null) throw new ArgumentNullException(nameof(bloque));

            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("sp_bloque_actualizar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", bloque.Id);
                cmd.Parameters.AddWithValue("@Identificador", bloque.Identificador);
                cmd.Parameters.AddWithValue("@NumeroPisos", bloque.NumeroPisos);
                cmd.Parameters.AddWithValue("@UnidadesPorPiso", bloque.UnidadesPorPiso);
                cmd.Parameters.AddWithValue("@Usuario", usuario ?? (object)DBNull.Value);

                var pRowVersion = new SqlParameter("@RowVersion", SqlDbType.Timestamp)
                {
                    Value = (object)bloque.RowVersion ?? DBNull.Value
                };
                cmd.Parameters.Add(pRowVersion);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Desactivar(int id, byte[] rowVersion, string usuario)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("sp_bloque_eliminar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Usuario", usuario ?? (object)DBNull.Value);

                var pRowVersion = new SqlParameter("@RowVersion", SqlDbType.Timestamp)
                {
                    Value = (object)rowVersion ?? DBNull.Value
                };
                cmd.Parameters.Add(pRowVersion);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private EBloque Mapear(SqlDataReader dr)
        {
            return new EBloque
            {
                Id = dr.GetInt32(dr.GetOrdinal("Id")),
                CondominioId = dr.GetInt32(dr.GetOrdinal("CondominioId")),
                Identificador = dr["Identificador"] as string,
                NumeroPisos = dr.GetInt32(dr.GetOrdinal("NumeroPisos")),
                UnidadesPorPiso = dr.GetInt32(dr.GetOrdinal("UnidadesPorPiso")),
                IsActive = dr.GetBoolean(dr.GetOrdinal("IsActive")),
                RowVersion = (byte[])dr["RowVersion"]
            };
        }
    }
}
