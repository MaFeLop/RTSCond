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

        public int Insertar(string nombre, int propietarioId, int unidadId, DateTime? fechaInicio, DateTime? fechaFin,
                            decimal porcentaje, bool esTitularPrincipal, string creador)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.", nameof(nombre));
            if (nombre.Length > 50) throw new ArgumentException("Nombre demasiado largo (máx. 50).", nameof(nombre));
            if (propietarioId <= 0) throw new ArgumentOutOfRangeException(nameof(propietarioId));
            if (unidadId <= 0) throw new ArgumentOutOfRangeException(nameof(unidadId));
            if (porcentaje < 0 || porcentaje > 100) throw new ArgumentOutOfRangeException(nameof(porcentaje));
            if (string.IsNullOrWhiteSpace(creador)) creador = "rtscon@local";

            return _dal.Insertar(nombre, propietarioId, unidadId, fechaInicio, fechaFin, porcentaje, esTitularPrincipal, creador);
        }

        public void Actualizar(int id, string nombre, int propietarioId, int unidadId, DateTime? fechaInicio, DateTime? fechaFin,
                               decimal porcentaje, bool esTitularPrincipal, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.", nameof(nombre));
            if (nombre.Length > 50) throw new ArgumentException("Nombre demasiado largo (máx. 50).", nameof(nombre));
            if (propietarioId <= 0) throw new ArgumentOutOfRangeException(nameof(propietarioId));
            if (unidadId <= 0) throw new ArgumentOutOfRangeException(nameof(unidadId));
            if (porcentaje < 0 || porcentaje > 100) throw new ArgumentOutOfRangeException(nameof(porcentaje));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Actualizar(id, nombre, propietarioId, unidadId, fechaInicio, fechaFin, porcentaje, esTitularPrincipal, rowVersion, editor);
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";
            _dal.Desactivar(id, rowVersion, editor);
        }
    }
}
