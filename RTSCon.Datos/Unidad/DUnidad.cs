// RTSCon.Datos\DUnidad.cs
using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DUnidad
    {
        private readonly string _cn;

        public DUnidad(string connectionString)
        {
            _cn = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // Listar unidades por bloque
        public DataTable ListarPorBloque(int bloqueId)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_unidad_listar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@BloqueId", SqlDbType.Int).Value = bloqueId;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt;
            }
        }

        // Buscar unidades (opcionalmente por bloque)
        public DataTable Buscar(int? bloqueId, string buscar, bool soloActivos, int top = 20)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_unidad_buscar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                da.SelectCommand.Parameters.Add("@BloqueId", SqlDbType.Int).Value =
                    (object)bloqueId ?? DBNull.Value;
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

        // Insertar unidad
        public int Insertar(
            int bloqueId,
            string numero,
            int piso,
            string tipologia,
            decimal? metros2,
            string estacionamiento,
            bool? amueblada,
            int? cantidadMuebles,
            decimal? cuotaMantenimientoEspecifica,
            string observaciones,
            string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_unidad_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@BloqueId", SqlDbType.Int).Value = bloqueId;
                cmd.Parameters.Add("@Numero", SqlDbType.NVarChar, 20).Value = (object)numero ?? DBNull.Value;
                cmd.Parameters.Add("@Piso", SqlDbType.Int).Value = piso;
                cmd.Parameters.Add("@Tipologia", SqlDbType.NVarChar, 50).Value = (object)tipologia ?? DBNull.Value;
                cmd.Parameters.Add("@Metros2", SqlDbType.Decimal).Value = (object)metros2 ?? DBNull.Value;
                cmd.Parameters.Add("@Estacionamiento", SqlDbType.NVarChar, 20).Value = (object)estacionamiento ?? DBNull.Value;
                cmd.Parameters.Add("@Amueblada", SqlDbType.Bit).Value = (object)amueblada ?? DBNull.Value;
                cmd.Parameters.Add("@CantidadMuebles", SqlDbType.Int).Value = (object)cantidadMuebles ?? DBNull.Value;
                cmd.Parameters.Add("@CuotaMantenimientoEspecifica", SqlDbType.Decimal).Value =
                    (object)cuotaMantenimientoEspecifica ?? DBNull.Value;
                cmd.Parameters.Add("@Observaciones", SqlDbType.NVarChar, 500).Value =
                    (object)observaciones ?? DBNull.Value;
                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                    (object)creador ?? DBNull.Value;

                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int);
                pId.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        // Actualizar unidad con RowVersion
        public void Actualizar(
            int id,
            string numero,
            int piso,
            string tipologia,
            decimal? metros2,
            string estacionamiento,
            bool? amueblada,
            int? cantidadMuebles,
            decimal? cuotaMantenimientoEspecifica,
            string observaciones,
            byte[] rowVersion,
            string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_unidad_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Numero", SqlDbType.NVarChar, 20).Value = (object)numero ?? DBNull.Value;
                cmd.Parameters.Add("@Piso", SqlDbType.Int).Value = piso;
                cmd.Parameters.Add("@Tipologia", SqlDbType.NVarChar, 50).Value = (object)tipologia ?? DBNull.Value;
                cmd.Parameters.Add("@Metros2", SqlDbType.Decimal).Value = (object)metros2 ?? DBNull.Value;
                cmd.Parameters.Add("@Estacionamiento", SqlDbType.NVarChar, 20).Value = (object)estacionamiento ?? DBNull.Value;
                cmd.Parameters.Add("@Amueblada", SqlDbType.Bit).Value = (object)amueblada ?? DBNull.Value;
                cmd.Parameters.Add("@CantidadMuebles", SqlDbType.Int).Value = (object)cantidadMuebles ?? DBNull.Value;
                cmd.Parameters.Add("@CuotaMantenimientoEspecifica", SqlDbType.Decimal).Value =
                    (object)cuotaMantenimientoEspecifica ?? DBNull.Value;
                cmd.Parameters.Add("@Observaciones", SqlDbType.NVarChar, 500).Value =
                    (object)observaciones ?? DBNull.Value;

                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                    (object)editor ?? DBNull.Value;
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value =
                    (object)rowVersion ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Desactivar (soft) unidad
        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_unidad_eliminar", cn))
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

        // PorId (DataRow)
        public DataRow PorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_unidad_obtener", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // BuscarPorId (DataTable) – mismo patrón que en Condominio/Bloque
        public DataTable BuscarPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_unidad_obtener", cn))
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
