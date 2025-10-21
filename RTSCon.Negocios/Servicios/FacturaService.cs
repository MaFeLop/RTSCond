using RTSCon.Negocios.Interfaces;
using RTSCon.Datos.Db;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Business.Services
{
    public class FacturaService : IFacturaService
    {
        public DataTable ListarFacturas(string periodo, int? condominioId, string estado, int page, int pageSize)
        {
            var ds = DbExecutor.ExecuteDataSet("dbo.sp_UI_FacturasPeriodo",
                    new SqlParameter("@Periodo", periodo),
                    new SqlParameter("@CondominioId", (object)condominioId ?? DBNull.Value),
                    new SqlParameter("@Estado", (object)estado ?? DBNull.Value),
                    new SqlParameter("@Page", page),
                    new SqlParameter("@PageSize", pageSize));

            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }
    }
}