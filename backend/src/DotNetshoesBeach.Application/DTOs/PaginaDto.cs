namespace DotNetshoesBeach.Application.DTOs;

/// <summary>Resultado paginado: os itens da página e o total de registros que atendem ao filtro.</summary>
public record PaginaDto<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int Total
)
{
    public int TotalPaginas => TamanhoPagina <= 0 ? 0 : (int)Math.Ceiling(Total / (double)TamanhoPagina);
}
