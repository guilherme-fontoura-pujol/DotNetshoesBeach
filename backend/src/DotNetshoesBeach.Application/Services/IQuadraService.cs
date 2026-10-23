using DotNetshoesBeach.Application.DTOs;

namespace DotNetshoesBeach.Application.Services;

public interface IQuadraService
{
    Task<IReadOnlyList<QuadraResponseDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<QuadraResponseDto> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuadraResponseDto> CriarAsync(SalvarQuadraDto dto, CancellationToken cancellationToken = default);
    Task<QuadraResponseDto> AtualizarAsync(Guid id, SalvarQuadraDto dto, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuadraResponseDto> ColocarEmManutencaoAsync(Guid id, ColocarEmManutencaoDto dto, CancellationToken cancellationToken = default);
    Task<QuadraResponseDto> DisponibilizarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeriodoOcupadoDto>> ListarOcupadosAsync(Guid id, DateTimeOffset de, DateTimeOffset ate, CancellationToken cancellationToken = default);
}
