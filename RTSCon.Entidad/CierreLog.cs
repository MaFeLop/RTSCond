using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Entidad
{
    public class CierreLog
    {
        public int LogId { get; set; }
        public string Periodo { get; set; } = "";
        public string Paso { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Mensaje { get; set; } = "";
        public DateTime FechaHora { get; set; }
    }
}
