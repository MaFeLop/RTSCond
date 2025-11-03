// DUnidad.cs
using System;
using System.Data;
using System.Data.SqlClient;
using RTSCon.Entidad;

namespace RTSCon.Datos
{
    public sealed class DUnidad
    {
        private readonly string _cn;
        public DUnidad(string connectionString) { _cn = connectionString; }

        private static SqlParameter P(string name, SqlDbType type, object value, int size, ParameterDirection dir)
        {
            var p = new SqlParameter(name, type);
            if (size > 0) p.Size = size;
            p.Direction = dir;
            p.Value = (value ?? DBNull.Value);
            return p;
        }

        public DataTable Listar(int bloqueId)
        {
            var dt = new DataTable();
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_unidad_listar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(P("@BloqueId", SqlDbType.Int, bloqueId, 0, ParameterDirection.Input));
                cn.Open();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
            }
            return dt;
        }

        public int Crear(EUnidad u, string usuario, out int nuevoId)
        {
            nuevoId = 0;
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_unidad_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(P("@BloqueId", SqlDbType.Int, u.BloqueId, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Numero", SqlDbType.NVarChar, u.Numero, 20, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Piso", SqlDbType.Int, u.Piso, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Tipologia", SqlDbType.NVarChar, u.Tipologia, 50, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Metros2", SqlDbType.Decimal, u.Metros2, 0, ParameterDirection.Input)).Precision = 10; cmd.Parameters["@Metros2"].Scale = 2;
                cmd.Parameters.Add(P("@Estacionamiento", SqlDbType.NVarChar, u.Estacionamiento, 20, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Amueblada", SqlDbType.Bit, u.Amueblada, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@CantidadMuebles", SqlDbType.Int, u.CantidadMuebles, 0, ParameterDirection.Input));
                var pCuota = P("@CuotaMantenimientoEspecifica", SqlDbType.Decimal, u.CuotaMantenimientoEspecifica, 0, ParameterDirection.Input);
                pCuota.Precision = 10; pCuota.Scale = 2; cmd.Parameters.Add(pCuota);
                cmd.Parameters.Add(P("@Observaciones", SqlDbType.NVarChar, u.Observaciones, 500, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Usuario", SqlDbType.NVarChar, usuario, 100, ParameterDirection.Input));
                var outId = P("@NuevoId", SqlDbType.Int, null, 0, ParameterDirection.Output);
                cmd.Parameters.Add(outId);
                cn.Open();
                cmd.ExecuteNonQuery();
                nuevoId = (outId.Value == DBNull.Value) ? 0 : Convert.ToInt32(outId.Value);
            }
            return nuevoId;
        }

        public void Actualizar(EUnidad u, string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_unidad_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(P("@Id", SqlDbType.Int, u.Id, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Numero", SqlDbType.NVarChar, u.Numero, 20, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Piso", SqlDbType.Int, u.Piso, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Tipologia", SqlDbType.NVarChar, u.Tipologia, 50, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Metros2", SqlDbType.Decimal, u.Metros2, 0, ParameterDirection.Input)).Precision = 10; cmd.Parameters["@Metros2"].Scale = 2;
                cmd.Parameters.Add(P("@Estacionamiento", SqlDbType.NVarChar, u.Estacionamiento, 20, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Amueblada", SqlDbType.Bit, u.Amueblada, 0, ParameterDirection.Input));
                cmd.Parameters.Add(P("@CantidadMuebles", SqlDbType.Int, u.CantidadMuebles, 0, ParameterDirection.Input));
                var pCuota = P("@CuotaMantenimientoEspecifica", SqlDbType.Decimal, u.CuotaMantenimientoEspecifica, 0, ParameterDirection.Input);
                pCuota.Precision = 10; pCuota.Scale = 2; cmd.Parameters.Add(pCuota);
                cmd.Parameters.Add(P("@Observaciones", SqlDbType.NVarChar, u.Observaciones, 500, ParameterDirection.Input));
                cmd.Parameters.Add(P("@Usuario", SqlDbType.NVarChar, usuario, 100, ParameterDirection.Input));
                cmd.Parameters.Add(P("@RowVersion", SqlDbType.Timestamp, u.RowVersion, 8, ParameterDirection.Input));
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int id, byte[] rowVersion, string usuario)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_unidad_eliminar", cn))
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
