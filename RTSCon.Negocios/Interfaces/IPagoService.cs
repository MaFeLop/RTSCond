using System.Data;

namespace RTSCon.Negocios.Interfaces
{
    public interface IPagoService
    {
        DataTable Listar(string periodo, int? condominioId, int pagina, int tamPagina);
    }
}
