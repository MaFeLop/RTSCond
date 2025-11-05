using RTSCon.Negocios.Interfaces;
using RTSCon.Datos;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Negocios.Servicios
{
    public class CobranzaService : ICobranzaService
    {
        public DataTable Listar(string periodo, int? condominioId, string nivel, int pagina, int tamPagina)
        {
            var ds = DbExecutor.ExecuteDataSet("dbo.sp_UI_DunningPeriodo",
                new SqlParameter("@Periodo", periodo),
                new SqlParameter("@CondominioId", (object)condominioId ?? DBNull.Value),
                new SqlParameter("@Nivel", (object)nivel ?? DBNull.Value),
                new SqlParameter("@Page", pagina),
                new SqlParameter("@PageSize", tamPagina));

            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }
    }
}
