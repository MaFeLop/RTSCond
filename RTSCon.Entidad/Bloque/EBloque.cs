using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// EBloque.cs
namespace RTSCon.Entidad
{
    public sealed class EBloque
    {
        public int Id { get; set; }
        public int CondominioId { get; set; }
        public string Identificador { get; set; }      // nvarchar(50)
        public int NumeroPisos { get; set; }
        public int UnidadesPorPiso { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }         // timestamp/rowversion
    }
}

