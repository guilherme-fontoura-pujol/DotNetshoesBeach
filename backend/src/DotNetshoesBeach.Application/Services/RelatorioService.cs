using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Exceptions;
using DotNetshoesBeach.Application.Interfaces;

namespace DotNetshoesBeach.Application.Services;

public class RelatorioService : IRelatorioService
{
    private static readonly TimeSpan PeriodoMaximo = TimeSpan.FromDays(366);

    private readonly IReservaRepository _reservaRepository;

    public RelatorioService(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public Task<IReadOnlyList<ResumoQuadraDto>> ResumoPorQuadraAsync(
        DateTimeOffset de, DateTimeOffset ate, CancellationToken cancellationToken = default)
    {
        if (ate <= de)
            throw new RegraDeNegocioException("O fim do período deve ser posterior ao início.");

        if (ate - de > PeriodoMaximo)
            throw new RegraDeNegocioException($"O período do relatório não pode passar de {PeriodoMaximo.TotalDays:0} dias.");

        return _reservaRepository.ResumirPorQuadraAsync(de.UtcDateTime, ate.UtcDateTime, cancellationToken);
    }
}
