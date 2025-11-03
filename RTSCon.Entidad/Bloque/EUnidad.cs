using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// EUnidad.cs
namespace RTSCon.Entidad
{
    public sealed class EUnidad
    {
        public int Id { get; set; }
        public int BloqueId { get; set; }
        public string Numero { get; set; }                 // nvarchar(20)
        public int Piso { get; set; }
        public string Tipologia { get; set; }              // nvarchar(50)
        public decimal? Metros2 { get; set; }
        public string Estacionamiento { get; set; }        // nvarchar(20)
        public bool? Amueblada { get; set; }
        public int? CantidadMuebles { get; set; }
        public decimal? CuotaMantenimientoEspecifica { get; set; }
        public string Observaciones { get; set; }          // nvarchar(500)
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }
    }
}

