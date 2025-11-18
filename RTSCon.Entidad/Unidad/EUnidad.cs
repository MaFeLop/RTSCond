// RTSCon.Entidad\EUnidad.cs
using System;

namespace RTSCon.Entidad
{
    public class EUnidad
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
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
