using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTSCon.Entidad;

namespace RTSCon.Datos.Propiedad
{
    public sealed class DPropiedad
    {
        private readonly string _cn;

        public DPropiedad(string connectionString)
        {
            _cn = connectionString;
        }

        // Devuelve (id, rowVersion) usando tupla (válido en C# 7.0+)
        public (int id, byte[] rowVersion) Crear(EPropiedad p)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_crear", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PropietarioId", p.PropietarioId);
                cmd.Parameters.AddWithValue("@UnidadId", p.UnidadId);
                cmd.Parameters.AddWithValue("@FechaInicio", p.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", (object)p.FechaFin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Porcentaje", p.Porcentaje);
                cmd.Parameters.AddWithValue("@EsTitularPrincipal", p.EsTitularPrincipal);
                cmd.Parameters.AddWithValue("@CreatedBy", p.UsuarioAuditoria ?? (object)DBNull.Value);

                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int);
                pId.Direction = ParameterDirection.Output;

                var pRv = cmd.Parameters.Add("@NuevaRowVersion", SqlDbType.VarBinary, 8);
                pRv.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                int id = (pId.Value == DBNull.Value) ? 0 : Convert.ToInt32(pId.Value);
                byte[] rv = (pRv.Value == DBNull.Value) ? new byte[0] : (byte[])pRv.Value;

                return (id, rv);
            }
        }

        public byte[] Actualizar(EPropiedad p)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_propiedad_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", p.Id);
                cmd.Parameters.AddWithValue("@PropietarioId", p.PropietarioId);
                cmd.Parameters.AddWithValue("@UnidadId", p.UnidadId);
                cmd.Parameters.AddWithValue("@FechaInicio", p.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", (object)p.FechaFin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Porcentaje", p.Porcentaje);
                cmd.Parameters.AddWithValue("@EsTitularPrincipal", p.EsTitularPrincipal);
                cmd.Parameters.AddWithValue("@IsActive", p.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", p.UsuarioAuditoria ?? (object)DBNull.Value);

                var pRow = cmd.Parameters.Add("@RowVersion", SqlDbType.VarBinary, 8);
                pRow.Value = p.RowVersion ?? (object)DBNull.Value;

                var pRvOut = cmd.Parameters.Add("@NuevaRowVersion", SqlDbType.VarBinary, 8);
                pRvOut.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                return (pRvOut.Value == DBNull.Value) ? new byte[0] : (byte[])pRvOut.Value;
            }
        }
        public DataRow ResumenPorUnidad(int unidadId)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_propiedad_resumen_unidad", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@UnidadId", unidadId);
                var dt = new DataTable(); da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

    }

    // Asegúrate de que tu entidad no use nullable reference types (string?):
    public sealed class EPropiedad
    {
        public int Id { get; set; }
        public int PropietarioId { get; set; }
        public int UnidadId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }           // OK (nullable value type)
        public decimal Porcentaje { get; set; }
        public bool EsTitularPrincipal { get; set; }
        public bool IsActive { get; set; }
        public string UsuarioAuditoria { get; set; }      // evita string?
        public byte[] RowVersion { get; set; }            // evita byte[]?
    }
}
