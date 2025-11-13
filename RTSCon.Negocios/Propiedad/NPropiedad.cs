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

        // Listado/Obtención (opcionales si ya los tienes en UI)
        public DataTable Listar(string buscar, bool soloActivas, int page, int pageSize, out int totalRows)
            => _dal.Listar(buscar, soloActivas, page, pageSize, out totalRows);

        public DataRow PorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.PorId(id);
        }

        // INSERTAR
        // Firma simple y clara; ajusta campos según tu tabla dbo.Propiedad
        public int Insertar(
            string nombre,
            string tipo,
            string ubicacion,
            decimal? cuotaMantenimientoBase,
            int? condominioId,
            string creador)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(creador)) creador = "rtscon@local";

            return _dal.Insertar(nombre, tipo, ubicacion, cuotaMantenimientoBase, condominioId, creador);
        }

        // ACTUALIZAR — firma completa (con concurrency)
        public void Actualizar(
            int id,
            string nombre,
            string tipo,
            string ubicacion,
            decimal? cuotaMantenimientoBase,
            int? condominioId,
            byte[] rowVersion,
            string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Actualizar(id, nombre, tipo, ubicacion, cuotaMantenimientoBase, condominioId, rowVersion, editor);
        }

        // ACTUALIZAR — overload “corto” (7 argumentos) para **hacer match con TU CÓDIGO EXISTENTE**
        // Internamente llama al anterior con rowVersion = null.
        public void Actualizar(
            int id,
            string nombre,
            string tipo,
            string ubicacion,
            decimal? cuotaMantenimientoBase,
            int? condominioId,
            string editor)
        {
            Actualizar(id, nombre, tipo, ubicacion, cuotaMantenimientoBase, condominioId, null, editor);
        }

        // DESACTIVAR (borrado lógico)
        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";
            _dal.Desactivar(id, rowVersion, editor);
        }

        // Overload conveniente si aún no pasas rowVersion desde UI
        public void Desactivar(int id, string editor)
        {
            Desactivar(id, null, editor);
        }

        // (Opcional) Notificación, si replicas patrón de correo que ya tienes en NCondominio
        public void NotificarAccion(int id, string accion, string usuarioEditor, string mailProfile)
        {
            _dal.NotificarAccion(id, accion, usuarioEditor, mailProfile);
        }
    }
}
