using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Entidad
{
    public class FacturaDto
    {
        public int FacturaId { get; set; }
        public string Periodo { get; set; } = "";
        public int CondominioId { get; set; }
        public int PropietarioId { get; set; }
        public string Estado { get; set; } = "";
        public decimal Total { get; set; }
        public string FechaCreacion { get; set; } = "";
    }
}
