using System;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public partial class NCondominio
    {
        public int Crear(CondominioCrearRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Nombre))
                throw new ArgumentException("El nombre del condominio es requerido.");

            return _dal.Insertar(req.Nombre, req.ReglamentoDocumentoId, req.Usuario);
        }
    }
}
