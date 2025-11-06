using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public partial class NCondominio
    {
        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            return _dal.Listar(buscar, soloActivos, page, pageSize, out totalRows);
        }
    }
}
