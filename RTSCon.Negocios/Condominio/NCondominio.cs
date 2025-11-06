using System;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public partial class NCondominio
    {
        private readonly DCondominio _dal;
        public NCondominio(DCondominio dal) { _dal = dal; }
    }

    public class CondominioCrearRequest
    {
        public string Nombre { get; set; }
        public int? ReglamentoDocumentoId { get; set; }
        public string Usuario { get; set; }
    }
}

