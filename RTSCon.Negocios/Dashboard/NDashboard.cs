using System;
using System.Data;
using RTSCon.Datos;
using System.Collections.Generic;

namespace RTSCon.Negocios
{
    public class DashboardResumen
    {
        public int CondominiosActivos { get; set; }
        public int BloquesActivos { get; set; }
        public int UnidadesActivas { get; set; }
        public int PropiedadesActivas { get; set; }
        public int UsuariosActivos { get; set; }
    }

    public class DashboardEvento
    {
        public string Modulo { get; set; }
        public string Accion { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class NDashboard
    {
        private readonly DDashboard _dal;

        public NDashboard(DDashboard dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public DashboardResumen ObtenerResumen()
        {
            DataRow row = _dal.ObtenerResumen();

            if (row == null)
                return new DashboardResumen();

            return new DashboardResumen
            {
                CondominiosActivos = ObtenerEntero(row, "CondominiosActivos"),
                BloquesActivos = ObtenerEntero(row, "BloquesActivos"),
                UnidadesActivas = ObtenerEntero(row, "UnidadesActivas"),
                PropiedadesActivas = ObtenerEntero(row, "PropiedadesActivas"),
                UsuariosActivos = ObtenerEntero(row, "UsuariosActivos")
            };
        }

        private int ObtenerEntero(DataRow row, string columna)
        {
            if (row == null)
                return 0;

            if (!row.Table.Columns.Contains(columna))
                return 0;

            if (row[columna] == DBNull.Value)
                return 0;

            return Convert.ToInt32(row[columna]);
        }

        public List<DashboardEvento> ObtenerUltimosEventos()
        {
            DataTable tabla = _dal.ObtenerUltimosEventos();
            List<DashboardEvento> eventos = new List<DashboardEvento>();

            foreach (DataRow row in tabla.Rows)
            {
                eventos.Add(new DashboardEvento
                {
                    Modulo = Convert.ToString(row["Modulo"]),
                    Accion = Convert.ToString(row["Accion"]),
                    Usuario = Convert.ToString(row["Usuario"]),
                    Fecha = Convert.ToDateTime(row["Fecha"])
                });
            }

            return eventos;
        }
    }
}