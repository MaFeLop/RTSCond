using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSCon.Datos
{
    public static class Conexion
    {
        public static string CadenaConexion =>
            ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
        // ↑ Usa aquí el NOMBRE real de tu connectionString en app.config
    }
}
