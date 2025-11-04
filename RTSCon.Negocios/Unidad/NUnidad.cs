using RTSCon.Datos;
using RTSCon.Entidad;
using RTSCon.Entidad.Unidad;
using System.Data;

// NUnidad.cs
public sealed class NUnidad
{
    private readonly DUnidad _dal;
    public NUnidad(string cn) => _dal = new DUnidad(cn);

    public DataTable Listar(int bloqueId) => _dal.Listar(bloqueId);

    public int Crear(EUnidad u, string usuario)
    {
        var ret = _dal.Crear(u, usuario, out var nuevoId);
        return (ret != 0) ? ret : nuevoId;
    }

    public void Actualizar(EUnidad u, string usuario) => _dal.Actualizar(u, usuario);

    public void Eliminar(int id, byte[] rowVersion, string usuario) => _dal.Eliminar(id, rowVersion, usuario);
}
