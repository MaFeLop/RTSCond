using RTSCon.Negocios.Interfaces;
using RTSCon.Datos;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Negocios.Servicios
{
    public class PeriodoService : IPeriodoService
    {
        public void CierreDeMes(string periodo, int? condominioId, bool dryRun, string ejecutor, int? ejecutorId)
        {
            DbExecutor.ExecuteNonQuery("dbo.sp_CierreDeMes_Maestro",
                    new SqlParameter("@Periodo", periodo),
                    new SqlParameter("@CondominioId", (object)condominioId ?? DBNull.Value),
                    new SqlParameter("@DryRun", dryRun ? 1 : 0),
                    new SqlParameter("@Ejecutor", (object)ejecutor ?? DBNull.Value),
                    new SqlParameter("@EjecutorId", (object)ejecutorId ?? DBNull.Value)
            );
        }

        public DataTable ObtenerCierreLog(string periodo)
        {
            var ds = DbExecutor.ExecuteDataSet("dbo.sp_ResumenPeriodo_Std",
                new SqlParameter("@Periodo", periodo));
            // Primer resultset suele ser resumen; el log lo traemos con SELECT directo:
            using (var cn = SqlConnectionFactory.Create())
            using (var cmd = new SqlCommand("SELECT * FROM dbo.CierreLog WHERE Periodo=@p ORDER BY LogId", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@p", periodo);
                var dt = new DataTable();
                cn.Open();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
