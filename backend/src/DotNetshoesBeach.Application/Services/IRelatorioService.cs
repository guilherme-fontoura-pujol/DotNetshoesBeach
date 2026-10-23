using DotNetshoesBeach.Application.DTOs;

namespace DotNetshoesBeach.Application.Services;

public interface IRelatorioService
{
    /// <summary>Ranking de quadras (mais reservadas primeiro) no período [de, ate).</summary>
    Task<IReadOnlyList<ResumoQuadraDto>> ResumoPorQuadraAsync(
        DateTimeOffset de, DateTimeOffset ate, CancellationToken cancellationToken = default);
}
