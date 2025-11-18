// RTSCon.Negocios\NUnidad.cs
using System;
using System.Data;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NUnidad
    {
        private readonly DUnidad _dal;

        public NUnidad(DUnidad dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // Listar por bloque
        public DataTable ListarPorBloque(int bloqueId)
        {
            if (bloqueId <= 0) throw new ArgumentOutOfRangeException(nameof(bloqueId));
            return _dal.ListarPorBloque(bloqueId);
        }

        // Buscar
        public DataTable Buscar(int? bloqueId, string buscar, bool soloActivos, int top = 20)
        {
            if (top <= 0) top = 20;
            return _dal.Buscar(bloqueId, buscar, soloActivos, top);
        }

        // Insertar
        public int Insertar(
            int bloqueId,
            string numero,
            int piso,
            string tipologia,
            decimal? metros2,
            string estacionamiento,
            bool? amueblada,
            int? cantidadMuebles,
            decimal? cuotaMantenimientoEspecifica,
            string observaciones,
            string creador)
        {
            if (bloqueId <= 0) throw new ArgumentOutOfRangeException(nameof(bloqueId));
            if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número requerido.", nameof(numero));
            if (piso < 0) throw new ArgumentOutOfRangeException(nameof(piso), "El piso no puede ser negativo.");
            if (string.IsNullOrWhiteSpace(tipologia)) throw new ArgumentException("Tipología requerida.", nameof(tipologia));
            if (amueblada == true && (!cantidadMuebles.HasValue || cantidadMuebles <= 0))
                throw new ArgumentException("Si la unidad está amueblada, la cantidad de muebles debe ser > 0.", nameof(cantidadMuebles));

            if (string.IsNullOrWhiteSpace(creador)) creador = "rtscon@local";

            return _dal.Insertar(
                bloqueId,
                numero,
                piso,
                tipologia,
                metros2,
                estacionamiento,
                amueblada,
                cantidadMuebles,
                cuotaMantenimientoEspecifica,
                observaciones,
                creador);
        }

        // Actualizar
        public void Actualizar(
            int id,
            string numero,
            int piso,
            string tipologia,
            decimal? metros2,
            string estacionamiento,
            bool? amueblada,
            int? cantidadMuebles,
            decimal? cuotaMantenimientoEspecifica,
            string observaciones,
            byte[] rowVersion,
            string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número requerido.", nameof(numero));
            if (piso < 0) throw new ArgumentOutOfRangeException(nameof(piso), "El piso no puede ser negativo.");
            if (string.IsNullOrWhiteSpace(tipologia)) throw new ArgumentException("Tipología requerida.", nameof(tipologia));
            if (amueblada == true && (!cantidadMuebles.HasValue || cantidadMuebles <= 0))
                throw new ArgumentException("Si la unidad está amueblada, la cantidad de muebles debe ser > 0.", nameof(cantidadMuebles));
            if (rowVersion == null || rowVersion.Length == 0)
                throw new ArgumentException("RowVersion requerida.", nameof(rowVersion));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Actualizar(
                id,
                numero,
                piso,
                tipologia,
                metros2,
                estacionamiento,
                amueblada,
                cantidadMuebles,
                cuotaMantenimientoEspecifica,
                observaciones,
                rowVersion,
                editor);
        }

        // Desactivar
        public void Desactivar(int id, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";
            _dal.Desactivar(id, null, editor);
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (rowVersion == null || rowVersion.Length == 0)
                throw new ArgumentException("RowVersion requerida.", nameof(rowVersion));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Desactivar(id, rowVersion, editor);
        }

        // PorId
        public DataRow PorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.PorId(id);
        }

        public DataTable BuscarPorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.BuscarPorId(id);
        }
    }
}
