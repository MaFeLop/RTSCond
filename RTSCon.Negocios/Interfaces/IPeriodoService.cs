using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Negocios.Interfaces
{
    public interface IPeriodoService
    {
        void CierreDeMes(string periodo, int? condominioId, bool dryRun, string ejecutor, int? ejecutorId);
        DataTable ObtenerCierreLog(string periodo);
        
    }
}
