using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Entidad
{
    public class PagoDto
    {
        public int PagoId { get; set; }
        public string Periodo { get; set; } = "";
        public int CondominioId { get; set; }
        public decimal Monto { get; set; }
        public string Fecha { get; set; } = "";
    }
}
