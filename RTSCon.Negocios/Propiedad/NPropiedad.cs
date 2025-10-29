using RTSCon.Datos.Propiedad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class NPropiedad
{
    private readonly DPropiedad _dal;
    public NPropiedad(string cn) => _dal = new DPropiedad(cn);

    public (int id, byte[] rowVersion) Crear(EPropiedad p) => _dal.Crear(p);
    public byte[] Actualizar(EPropiedad p) => _dal.Actualizar(p);
}

