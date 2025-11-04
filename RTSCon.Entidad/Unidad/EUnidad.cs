using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Entidad.Unidad
{
    public sealed class EUnidad
    {
        public int Id { get; set; }
        public int BloqueId { get; set; }
        public string Numero { get; set; }
        public int Piso { get; set; }
        public string Tipologia { get; set; }
        public decimal? Metros2 { get; set; }
        public string Estacionamiento { get; set; }
        public bool? Amueblada { get; set; }
        public int? CantidadMuebles { get; set; }
        public decimal? CuotaMantenimientoEspecifica { get; set; }
        public string Observaciones { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }

    }
}
