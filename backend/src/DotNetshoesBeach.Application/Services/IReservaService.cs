using DotNetshoesBeach.Application.Common;
using DotNetshoesBeach.Application.DTOs;

namespace DotNetshoesBeach.Application.Services;

public interface IReservaService
{
    Task<ReservaResponseDto> CriarAsync(CriarReservaDto dto, UsuarioAtual usuario, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReservaResponseDto>> ListarDoUsuarioAsync(UsuarioAtual usuario, CancellationToken cancellationToken = default);
    Task<PaginaDto<ReservaResponseDto>> ListarAsync(FiltroReservaDto filtro, CancellationToken cancellationToken = default);
    Task<ReservaResponseDto> CancelarAsync(Guid id, UsuarioAtual usuario, CancellationToken cancellationToken = default);
}
