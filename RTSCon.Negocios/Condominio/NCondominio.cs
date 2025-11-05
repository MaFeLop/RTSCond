using System;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NCondominio
    {
        private readonly DCondominio _dal;
        public NCondominio(DCondominio dal) { _dal = dal; }

        public int Crear(CondominioCrearRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Nombre))
                throw new ArgumentException("El nombre del condominio es requerido.");

            // aquí podrías validar que el Documento exista/esté activo si viene Id
            return _dal.Insertar(req);
        }
    }
}
