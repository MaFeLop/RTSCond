using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Entidad
{
    public class DunningDto
    {
        public int DunningId { get; set; }
        public string Periodo { get; set; } = "";
        public string Nivel { get; set; } = "";
        public int CondominioId { get; set; }
        public string FechaCreacion { get; set; } = "";
    }
}
