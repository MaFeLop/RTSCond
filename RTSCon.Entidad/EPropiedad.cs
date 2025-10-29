using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Entidad
{
    public sealed class EPropiedad
    {
        public int Id { get; set; }
        public int PropietarioId { get; set; }
        public int UnidadId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal Porcentaje { get; set; }          // 0..100
        public bool EsTitularPrincipal { get; set; }
        public bool IsActive { get; set; }
        public string UsuarioAuditoria { get; set; } = "";
        public byte[] RowVersion { get; set; }            // para Update
    }

}
