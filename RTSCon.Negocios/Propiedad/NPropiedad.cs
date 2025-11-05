using System;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NPropiedad
    {
        private readonly DPropiedad _dal;
        public NPropiedad(DPropiedad dal) { _dal = dal; }

        public int Crear(int unidadId, int propietarioId, decimal porcentaje, bool esTitularPrincipal, string usuario)
        {
            // Validaciones de entrada básicas
            if (unidadId <= 0) throw new ArgumentException("Unidad inválida.");
            if (propietarioId <= 0) throw new ArgumentException("Propietario inválido.");
            if (porcentaje <= 0m || porcentaje > 100m) throw new ArgumentException("Porcentaje fuera de rango.");
            porcentaje = Math.Round(porcentaje, 2, MidpointRounding.AwayFromZero);

            return _dal.Insertar(unidadId, propietarioId, porcentaje, esTitularPrincipal, usuario);
        }

        public void Actualizar(int id, int unidadId, int propietarioId, decimal porcentaje, bool esTitularPrincipal, string usuario, byte[] rowVersion = null)
        {
            if (id <= 0) throw new ArgumentException("Id inválido.");
            if (unidadId <= 0) throw new ArgumentException("Unidad inválida.");
            if (propietarioId <= 0) throw new ArgumentException("Propietario inválido.");
            if (porcentaje <= 0m || porcentaje > 100m) throw new ArgumentException("Porcentaje fuera de rango.");
            porcentaje = Math.Round(porcentaje, 2, MidpointRounding.AwayFromZero);

            _dal.Actualizar(id, unidadId, propietarioId, porcentaje, esTitularPrincipal, usuario, rowVersion);
        }

        public void Desactivar(int id, string usuario, byte[] rowVersion = null)
        {
            if (id <= 0) throw new ArgumentException("Id inválido.");
            _dal.Desactivar(id, usuario, rowVersion);
        }
    }
}
