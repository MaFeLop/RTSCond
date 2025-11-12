using RTSCon.Datos;
using System;
using System.Data;

namespace RTSCon.Negocios
{
    public class NCondominio
    {
        private readonly DCondominio _dal;
        public NCondominio(DCondominio dal) { _dal = dal; }

        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, out int totalRows)
            => _dal.Listar(buscar, soloActivos, page, pageSize, out totalRows);

        public DataRow PorId(int id) => _dal.PorId(id);

        public int Insertar(
    string nombre, string direccion, string tipo, string adminResp,
    DateTime fechaConstitucion, decimal cuotaBase,
    string emailNotif, bool enviarNotifProp,
    int? reglamentoDocId, int? idPropietario, int? idSec1, int? idSec2, int? idSec3,
    string creador)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.");
            if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("Dirección requerida.");
            if (string.IsNullOrWhiteSpace(tipo)) throw new ArgumentException("Tipo requerido.");
            if (string.IsNullOrWhiteSpace(adminResp)) throw new ArgumentException("Administrador responsable requerido.");
            if (fechaConstitucion == default(DateTime)) throw new ArgumentException("Fecha de constitución requerida.");
            if (cuotaBase < 0) throw new ArgumentException("La cuota base no puede ser negativa.");
            if (string.IsNullOrWhiteSpace(emailNotif) || !emailNotif.Contains("@"))
                throw new ArgumentException("Correo de notificaciones inválido.");

            return _dal.Insertar(
                nombre.Trim(), direccion.Trim(), tipo.Trim(), adminResp.Trim(),
                fechaConstitucion.Date, cuotaBase,
                emailNotif.Trim(), enviarNotifProp,
                reglamentoDocId, idPropietario, idSec1, idSec2, idSec3,
                creador);
        }


        public void Actualizar(int id, string nombre, int? reglamentoDocId, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentException("Id inválido.");
            if (rowVersion == null || rowVersion.Length == 0) throw new ArgumentException("RowVersion requerido.");
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.");
            _dal.Actualizar(id, nombre.Trim(), reglamentoDocId, rowVersion, editor);
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentException("Id inválido.");
            if (rowVersion == null || rowVersion.Length == 0) throw new ArgumentException("RowVersion requerido.");
            _dal.Desactivar(id, rowVersion, editor);
        }
    }

    public class CondominioCrearRequest
    {
        public string Nombre { get; set; }
        public int? ReglamentoDocumentoId { get; set; }
        public string Usuario { get; set; }
    }
}

