using System;
using System.Data;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NPropiedad
    {
        private readonly DPropiedad _dal;
        public NPropiedad(DPropiedad dal) => _dal = dal ?? throw new ArgumentNullException(nameof(dal));

        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows, int? ownerId = null)
            => _dal.Listar(buscar, soloActivos, page, pageSize, out totalRows, ownerId);

        public DataRow PorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.PorId(id);
        }

        public int Insertar(int propietarioId, int unidadId, DateTime? fechaInicio, DateTime? fechaFin,
                            decimal porcentaje, bool esTitularPrincipal, string creador)
        {
            if (propietarioId <= 0) throw new ArgumentOutOfRangeException(nameof(propietarioId));
            if (unidadId <= 0) throw new ArgumentOutOfRangeException(nameof(unidadId));
            if (porcentaje < 0 || porcentaje > 100) throw new ArgumentOutOfRangeException(nameof(porcentaje));
            if (string.IsNullOrWhiteSpace(creador)) creador = "rtscon@local";

            return _dal.Insertar(propietarioId, unidadId, fechaInicio, fechaFin, porcentaje, esTitularPrincipal, creador);
        }

        public void Actualizar(int id, int propietarioId, int unidadId, DateTime? fechaInicio, DateTime? fechaFin,
                               decimal porcentaje, bool esTitularPrincipal, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (propietarioId <= 0) throw new ArgumentOutOfRangeException(nameof(propietarioId));
            if (unidadId <= 0) throw new ArgumentOutOfRangeException(nameof(unidadId));
            if (porcentaje < 0 || porcentaje > 100) throw new ArgumentOutOfRangeException(nameof(porcentaje));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Actualizar(id, propietarioId, unidadId, fechaInicio, fechaFin, porcentaje, esTitularPrincipal, rowVersion, editor);
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";
            _dal.Desactivar(id, rowVersion, editor);
        }
    }
}
