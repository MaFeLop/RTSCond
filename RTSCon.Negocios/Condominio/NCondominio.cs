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

        public int Insertar(string nombre, int? reglamentoDocId, string creador)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido.");
            return _dal.Insertar(nombre.Trim(), reglamentoDocId, creador);
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

