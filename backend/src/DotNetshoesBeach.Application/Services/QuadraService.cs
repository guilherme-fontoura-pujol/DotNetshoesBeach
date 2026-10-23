using DotNetshoesBeach.Application.Common;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Exceptions;
using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;

namespace DotNetshoesBeach.Application.Services;

public class QuadraService : IQuadraService
{
    private static readonly TimeSpan JanelaMaximaConsulta = TimeSpan.FromDays(62);

    private readonly IQuadraRepository _quadraRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _clock;

    public QuadraService(
        IQuadraRepository quadraRepository,
        IReservaRepository reservaRepository,
        IUnitOfWork unitOfWork,
        TimeProvider clock)
    {
        _quadraRepository = quadraRepository;
        _reservaRepository = reservaRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<IReadOnlyList<QuadraResponseDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var quadras = await _quadraRepository.ListarAsync(cancellationToken);
        return quadras.Select(q => q.ParaDto()).ToList();
    }

    public async Task<QuadraResponseDto> ObterAsync(Guid id, CancellationToken cancellationToken = default) =>
        (await ObterOuFalharAsync(id, cancellationToken)).ParaDto();

    public async Task<QuadraResponseDto> CriarAsync(SalvarQuadraDto dto, CancellationToken cancellationToken = default)
    {
        var quadra = new Quadra(dto.Nome, dto.Descricao, dto.ValorHora);

        if (await _quadraRepository.ExisteNomeAsync(quadra.Nome, null, cancellationToken))
            throw new ConflitoException("Já existe uma quadra com esse nome.");

        await _quadraRepository.AdicionarAsync(quadra, cancellationToken);
        await _unitOfWork.SalvarAsync(cancellationToken);
        return quadra.ParaDto();
    }

    public async Task<QuadraResponseDto> AtualizarAsync(Guid id, SalvarQuadraDto dto, CancellationToken cancellationToken = default)
    {
        var quadra = await ObterOuFalharAsync(id, cancellationToken);
        quadra.Atualizar(dto.Nome, dto.Descricao, dto.ValorHora);

        if (await _quadraRepository.ExisteNomeAsync(quadra.Nome, quadra.Id, cancellationToken))
            throw new ConflitoException("Já existe uma quadra com esse nome.");

        await _unitOfWork.SalvarAsync(cancellationToken);
        return quadra.ParaDto();
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var quadra = await ObterOuFalharAsync(id, cancellationToken);

        // CountAsync: o banco só devolve o número, sem carregar as reservas.
        var qtdAtivas = await _reservaRepository.ContarAtivasDaQuadraAsync(id, _clock.GetUtcNow().UtcDateTime, cancellationToken);
        if (qtdAtivas > 0)
            throw new RegraDeNegocioException(
                $"A quadra possui {qtdAtivas} reserva(s) ativa(s) ou futura(s). Cancele-as ou coloque a quadra em manutenção.");

        // O histórico de reservas antigas/canceladas é removido junto (cascade no banco).
        _quadraRepository.Remover(quadra);
        await _unitOfWork.SalvarAsync(cancellationToken);
    }

    public async Task<QuadraResponseDto> ColocarEmManutencaoAsync(
        Guid id, ColocarEmManutencaoDto dto, CancellationToken cancellationToken = default)
    {
        var quadra = await ObterOuFalharAsync(id, cancellationToken);
        var agora = _clock.GetUtcNow().UtcDateTime;

        var ativas = await _reservaRepository.ListarAtivasDaQuadraAsync(id, agora, cancellationToken);
        if (ativas.Count > 0)
        {
            if (!dto.CancelarReservas)
                throw new ConflitoException(
                    $"A quadra possui {ativas.Count} reserva(s) ativa(s) ou futura(s). " +
                    "Reenvie com \"cancelarReservas\": true para cancelá-las e iniciar a manutenção.");

            foreach (var reserva in ativas)
                reserva.Cancelar(agora, permitirJaIniciada: true);
        }

        quadra.ColocarEmManutencao(dto.Motivo);
        await _unitOfWork.SalvarAsync(cancellationToken);
        return quadra.ParaDto();
    }

    public async Task<QuadraResponseDto> DisponibilizarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var quadra = await ObterOuFalharAsync(id, cancellationToken);
        quadra.Disponibilizar();
        await _unitOfWork.SalvarAsync(cancellationToken);
        return quadra.ParaDto();
    }

    public async Task<IReadOnlyList<PeriodoOcupadoDto>> ListarOcupadosAsync(
        Guid id, DateTimeOffset de, DateTimeOffset ate, CancellationToken cancellationToken = default)
    {
        if (ate <= de)
            throw new RegraDeNegocioException("O fim do período deve ser posterior ao início.");

        if (ate - de > JanelaMaximaConsulta)
            throw new RegraDeNegocioException($"O período consultado não pode passar de {JanelaMaximaConsulta.TotalDays:0} dias.");

        await ObterOuFalharAsync(id, cancellationToken);

        var reservas = await _reservaRepository.ListarConfirmadasNoPeriodoAsync(
            id, de.UtcDateTime, ate.UtcDateTime, cancellationToken);

        return reservas
            .OrderBy(r => r.DataHoraInicio)
            .Select(r => new PeriodoOcupadoDto(r.DataHoraInicio, r.DataHoraFim))
            .ToList();
    }

    private async Task<Quadra> ObterOuFalharAsync(Guid id, CancellationToken cancellationToken) =>
        await _quadraRepository.ObterPorIdAsync(id, cancellationToken)
        ?? throw new NaoEncontradoException("Quadra não encontrada.");
}
