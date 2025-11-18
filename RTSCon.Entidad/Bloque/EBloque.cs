// RTSCon.Entidad\EBloque.cs
using System;

namespace RTSCon.Entidad
{
    public class EBloque
    {
        public int Id { get; set; }
        public int CondominioId { get; set; }
        public string Identificador { get; set; }
        public int NumeroPisos { get; set; }
        public int UnidadesPorPiso { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
