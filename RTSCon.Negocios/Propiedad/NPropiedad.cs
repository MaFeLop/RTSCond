using System;
using System.Data;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NPropiedad
    {
        private readonly DPropiedad _dal;

        public NPropiedad(DPropiedad dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows, int? ownerId = null)
        {
            return _dal.Listar(buscar, soloActivos, page, pageSize, out totalRows, ownerId);
        }

        public DataRow PorId(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            return _dal.PorId(id);
        }

        public int Insertar(
            string nombre,
            int propietarioId,
            int unidadId,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            decimal porcentaje,
            bool esTitularPrincipal,
            string tipoTenencia,
            string creador)
        {
            ValidarDatosBase(nombre, propietarioId, unidadId, porcentaje, tipoTenencia, fechaInicio, fechaFin);

            if (string.IsNullOrWhiteSpace(creador))
                creador = "rtscon@local";

            if (tipoTenencia == "Comprada")
                fechaFin = null;

            return _dal.Insertar(
                nombre.Trim(),
                propietarioId,
                unidadId,
                fechaInicio,
                fechaFin,
                porcentaje,
                esTitularPrincipal,
                tipoTenencia,
                creador);
        }

        public void Actualizar(
            int id,
            string nombre,
            int propietarioId,
            int unidadId,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            decimal porcentaje,
            bool esTitularPrincipal,
            byte[] rowVersion,
            string tipoTenencia,
            string editor)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            if (rowVersion == null || rowVersion.Length == 0)
                throw new ArgumentException("RowVersion requerida.", nameof(rowVersion));

            ValidarDatosBase(nombre, propietarioId, unidadId, porcentaje, tipoTenencia, fechaInicio, fechaFin);

            if (string.IsNullOrWhiteSpace(editor))
                editor = "rtscon@local";

            if (tipoTenencia == "Comprada")
                fechaFin = null;

            _dal.Actualizar(
                id,
                nombre.Trim(),
                propietarioId,
                unidadId,
                fechaInicio,
                fechaFin,
                porcentaje,
                esTitularPrincipal,
                rowVersion,
                tipoTenencia,
                editor);
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            if (string.IsNullOrWhiteSpace(editor))
                editor = "rtscon@local";

            _dal.Desactivar(id, rowVersion, editor);
        }

        private void ValidarDatosBase(
            string nombre,
            int propietarioId,
            int unidadId,
            decimal porcentaje,
            string tipoTenencia,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Nombre requerido.", nameof(nombre));

            if (nombre.Trim().Length > 50)
                throw new ArgumentException("Nombre demasiado largo (máx. 50).", nameof(nombre));

            if (propietarioId <= 0)
                throw new ArgumentOutOfRangeException(nameof(propietarioId));

            if (unidadId <= 0)
                throw new ArgumentOutOfRangeException(nameof(unidadId));

            if (porcentaje <= 0 || porcentaje > 100)
                throw new ArgumentOutOfRangeException(nameof(porcentaje), "El porcentaje debe estar entre 0.01 y 100.");

            if (tipoTenencia != "Comprada" && tipoTenencia != "Rentada")
                throw new ArgumentException("Tipo de tenencia inválido.", nameof(tipoTenencia));

            if (tipoTenencia == "Rentada")
            {
                if (!fechaInicio.HasValue || !fechaFin.HasValue)
                    throw new ArgumentException("Para una propiedad rentada, la fecha de inicio y terminación son obligatorias.");

                if (fechaFin.Value.Date <= fechaInicio.Value.Date)
                    throw new ArgumentException("La fecha de terminación debe ser posterior a la fecha de inicio.");
            }
        }
    }
}