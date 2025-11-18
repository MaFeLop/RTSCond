// RTSCon.Negocios\NBloque.cs
using System;
using System.Data;
using RTSCon.Datos;

namespace RTSCon.Negocios
{
    public class NBloque
    {
        private readonly DBloque _dal;

        public NBloque(DBloque dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // Listar por condominio (puedes usar esto para llenar combos o grids simples)
        public DataTable ListarPorCondominio(int condominioId)
        {
            if (condominioId <= 0) throw new ArgumentOutOfRangeException(nameof(condominioId));
            return _dal.ListarPorCondominio(condominioId);
        }

        // Búsqueda con filtro de texto y soloActivos
        public DataTable Buscar(int? condominioId, string buscar, bool soloActivos, int top = 20)
        {
            if (top <= 0) top = 20;
            return _dal.Buscar(condominioId, buscar, soloActivos, top);
        }

        // INSERTAR
        public int Insertar(int condominioId, string identificador, int numeroPisos, int unidadesPorPiso, string creador)
        {
            if (condominioId <= 0) throw new ArgumentOutOfRangeException(nameof(condominioId));
            if (string.IsNullOrWhiteSpace(identificador)) throw new ArgumentException("Identificador requerido.", nameof(identificador));
            if (numeroPisos <= 0) throw new ArgumentOutOfRangeException(nameof(numeroPisos), "El número de pisos debe ser > 0.");
            if (unidadesPorPiso <= 0) throw new ArgumentOutOfRangeException(nameof(unidadesPorPiso), "Las unidades por piso deben ser > 0.");
            if (string.IsNullOrWhiteSpace(creador)) creador = "rtscon@local";

            return _dal.Insertar(condominioId, identificador, numeroPisos, unidadesPorPiso, creador);
        }

        // ACTUALIZAR
        public void Actualizar(
            int id,
            string identificador,
            int numeroPisos,
            int unidadesPorPiso,
            byte[] rowVersion,
            string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(identificador)) throw new ArgumentException("Identificador requerido.", nameof(identificador));
            if (numeroPisos <= 0) throw new ArgumentOutOfRangeException(nameof(numeroPisos), "El número de pisos debe ser > 0.");
            if (unidadesPorPiso <= 0) throw new ArgumentOutOfRangeException(nameof(unidadesPorPiso), "Las unidades por piso deben ser > 0.");
            if (rowVersion == null || rowVersion.Length == 0) throw new ArgumentException("RowVersion requerida.", nameof(rowVersion));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Actualizar(id, identificador, numeroPisos, unidadesPorPiso, rowVersion, editor);
        }

        // DESACTIVAR (overload sin RowVersion por si lo necesitas)
        public void Desactivar(int id, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";
            _dal.Desactivar(id, null, editor);
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (rowVersion == null || rowVersion.Length == 0) throw new ArgumentException("RowVersion requerida.", nameof(rowVersion));
            if (string.IsNullOrWhiteSpace(editor)) editor = "rtscon@local";

            _dal.Desactivar(id, rowVersion, editor);
        }

        // PorId (DataRow)
        public DataRow PorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.PorId(id);
        }

        // BuscarPorId (DataTable), útil para UpdateBloque si copias patrón de UpdateCondominio
        public DataTable BuscarPorId(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _dal.BuscarPorId(id);
        }
    }
}
