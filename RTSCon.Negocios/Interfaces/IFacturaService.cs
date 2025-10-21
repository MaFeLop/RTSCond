using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Negocios.Interfaces
{
    public interface IFacturaService
    {
        DataTable ListarFacturas(string periodo, int? condominioId, string estado, int page, int pageSize);
    
    }
}
