using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Entidad
{
    public class ECondominio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? ReglamentoDocumentoId { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
