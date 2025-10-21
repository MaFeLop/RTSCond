using System.Data;

namespace RTSCon.Negocios.Interfaces
{
    public interface ICobranzaService
    {
        DataTable Listar(string periodo, int? condominioId, string nivel, int pagina, int tamPagina);
    }
}