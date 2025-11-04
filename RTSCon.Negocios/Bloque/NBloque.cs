using RTSCon.Datos;
using RTSCon.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Negocios.Bloque
{
    public sealed class NBloque
    {
        private readonly DBloque _dal;
        public NBloque(string cn) => _dal = new DBloque(cn);

        public DataTable Listar(int condominioId) => _dal.Listar(condominioId);

        public int Crear(EBloque b, string usuario)
        {

            var ret = _dal.Crear(b, usuario, out var nuevoId);
            return (ret != 0) ? ret : nuevoId;
        }

        public void Actualizar(EBloque b, string usuario) => _dal.Actualizar(b, usuario);

        public void Eliminar(int id, byte[] rowVersion, string usuario) => _dal.Eliminar(id, rowVersion, usuario);
    }
}
