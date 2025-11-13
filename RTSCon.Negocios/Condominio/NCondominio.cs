using System;
using System.Data;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NCondominio
    {
        private readonly DCondominio _dal;
        public NCondominio(DCondominio dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // --------- LISTAR (paginado) ----------
        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows)
        {
            int? ownerId = null;
            var rol = UserContext.Rol?.Trim();
            if (rol == "Propietario") ownerId = UserContext.UsuarioAuthId;   // 🔒

            return _dal.Listar(buscar, soloActivos, page, pageSize, ownerId, out totalRows);
        }

        // --------- NOTIFICAR ACCIÓN SIMPLE (mediante correo) ----------
        public void NotificarAccionSimple(string to, string asunto, string cuerpoHtml, string mailProfile)
    => _dal.Notificar(to, asunto, cuerpoHtml, mailProfile);


        // --------- INSERTAR ----------
        public int Insertar(
            string nombre, string direccion, string tipo, string adminResp,
            DateTime fechaConstitucion, decimal cuotaBase,
            string emailNotif, bool enviarNotifProp,
            int? reglamentoDocId, int? idPropietario, int? idSec1, int? idSec2, int? idSec3,
            string creador)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("Dirección requerida.", nameof(direccion));
            if (string.IsNullOrWhiteSpace(tipo)) throw new ArgumentException("Tipo requerido.", nameof(tipo));
            if (string.IsNullOrWhiteSpace(adminResp)) throw new ArgumentException("Administrador responsable requerido.", nameof(adminResp));
            if (string.IsNullOrWhiteSpace(creador)) creador = "rtscon@local";

            return _dal.Insertar(
                nombre, direccion, tipo, adminResp,
                fechaConstitucion, cuotaBase,
                emailNotif, enviarNotifProp,
                reglamentoDocId, idPropietario, idSec1, idSec2, idSec3,
                creador
            );
        }

        // --------- OBTENER POR ID (DataTable, lo usa UpdateCondominio) ----------
        public DataTable BuscarPorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.BuscarPorId(id);
        }

        // (Opcional si en algún lado quieres 1 sola fila)
        public DataRow PorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.PorId(id);
        }

        // --------- ACTUALIZAR (firma de 16 parámetros + RowVersion + editor) ----------
        public void Actualizar(
            int id,
            string nombre, string direccion, string tipo, string administradorResponsable,
            DateTime fechaConstitucion, decimal cuotaMantenimientoBase,
            string emailNotificaciones, bool enviarNotifsAlPropietario,
            int? reglamentoDocumentoId, int? idPropietario, int? idSecretario1, int? idSecretario2, int? idSecretario3,
            byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("Dirección requerida.", nameof(direccion));
            if (string.IsNullOrWhiteSpace(tipo)) throw new ArgumentException("Tipo requerido.", nameof(tipo));
            if (string.IsNullOrWhiteSpace(administradorResponsable)) throw new ArgumentException("Administrador requerido.", nameof(administradorResponsable));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Actualizar(
                id,
                nombre, direccion, tipo, administradorResponsable,
                fechaConstitucion, cuotaMantenimientoBase,
                emailNotificaciones, enviarNotifsAlPropietario,
                reglamentoDocumentoId, idPropietario, idSecretario1, idSecretario2, idSecretario3,
                rowVersion, editor
            );
        }

        // --------- DESACTIVAR ----------
        // Overload conveniente para que compile tu llamada _neg.Desactivar(id, usuario)
        public void Desactivar(int id, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";
            _dal.Desactivar(id, null, editor);
        }

        // Overload completo por si luego manejas concurrencia
        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";
            _dal.Desactivar(id, rowVersion, editor);
        }

        // --------- BÚSQUEDA RÁPIDA (top N) ----------
        public DataTable Buscar(string buscar, bool soloActivos, int top = 20)
            => _dal.Buscar(buscar, soloActivos, top);

        // RTSCon.Negocios\NCondominio.cs  (añade)
        public void NotificarAccion(int id, string accion, string editor, string mailProfile)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            _dal.NotificarAccion(id, accion, editor, mailProfile);
        }

    }
}
