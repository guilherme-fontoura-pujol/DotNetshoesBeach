using DotNetshoesBeach.Application.Common;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Exceptions;
using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;

namespace DotNetshoesBeach.Application.Services;

public class ReservaService : IReservaService
{
    private const int TamanhoMaximoPagina = 100;

    private readonly IReservaRepository _reservaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IQuadraRepository _quadraRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _clock;

    public ReservaService(
        IReservaRepository reservaRepository,
        IClienteRepository clienteRepository,
        IQuadraRepository quadraRepository,
        IUnitOfWork unitOfWork,
        TimeProvider clock)
    {
        _reservaRepository = reservaRepository;
        _clienteRepository = clienteRepository;
        _quadraRepository = quadraRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<ReservaResponseDto> CriarAsync(
        CriarReservaDto dto, UsuarioAtual usuario, CancellationToken cancellationToken = default)
    {
        // Cliente comum só reserva para si; admin pode reservar em nome de qualquer cliente.
        var clienteId = dto.ClienteId ?? usuario.Id;
        if (clienteId != usuario.Id && !usuario.EhAdmin)
            throw new AcessoNegadoException("Você só pode reservar quadras em seu próprio nome.");

        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId, cancellationToken)
            ?? throw new NaoEncontradoException("Cliente não encontrado.");

        var quadra = await _quadraRepository.ObterPorIdAsync(dto.QuadraId, cancellationToken)
            ?? throw new NaoEncontradoException("Quadra não encontrada.");

        if (!quadra.EstaDisponivel)
            throw new RegraDeNegocioException("Esta quadra está em manutenção e não pode ser reservada.");

        var inicio = dto.DataHoraInicio.UtcDateTime;
        var fim = dto.DataHoraFim.UtcDateTime;

        var reserva = new Reserva(cliente.Id, quadra.Id, inicio, fim, quadra.ValorHora, _clock.GetUtcNow().UtcDateTime);

        // Checagem amigável. A garantia real contra corrida (dois pedidos simultâneos)
        // é a constraint de exclusão no PostgreSQL, traduzida pelo IUnitOfWork.
        if (await _reservaRepository.ExisteConflitoAsync(quadra.Id, inicio, fim, cancellationToken))
            throw new ConflitoException("Já existe uma reserva para esta quadra nesse horário.");

        await _reservaRepository.AdicionarAsync(reserva, cancellationToken);
        await _unitOfWork.SalvarAsync(cancellationToken);

        return reserva.ParaDto(cliente.Nome, quadra.Nome);
    }

    public Task<IReadOnlyList<ReservaResponseDto>> ListarDoUsuarioAsync(
        UsuarioAtual usuario, CancellationToken cancellationToken = default) =>
        _reservaRepository.ListarDoClienteAsync(usuario.Id, cancellationToken);

    public Task<PaginaDto<ReservaResponseDto>> ListarAsync(
        FiltroReservaDto filtro, CancellationToken cancellationToken = default)
    {
        if (filtro.Pagina < 1)
            throw new RegraDeNegocioException("A página deve ser maior ou igual a 1.");

        if (filtro.TamanhoPagina < 1 || filtro.TamanhoPagina > TamanhoMaximoPagina)
            throw new RegraDeNegocioException($"O tamanho da página deve ficar entre 1 e {TamanhoMaximoPagina}.");

        return _reservaRepository.ListarPaginadoAsync(
            filtro.QuadraId, filtro.ClienteId, filtro.De?.UtcDateTime, filtro.Ate?.UtcDateTime, filtro.Status,
            filtro.Pagina, filtro.TamanhoPagina, cancellationToken);
    }

    public async Task<ReservaResponseDto> CancelarAsync(
        Guid id, UsuarioAtual usuario, CancellationToken cancellationToken = default)
    {
        var reserva = await _reservaRepository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Reserva não encontrada.");

        if (!usuario.EhAdmin && reserva.ClienteId != usuario.Id)
            throw new AcessoNegadoException("Você só pode cancelar as suas próprias reservas.");

        reserva.Cancelar(_clock.GetUtcNow().UtcDateTime, permitirJaIniciada: usuario.EhAdmin);
        await _unitOfWork.SalvarAsync(cancellationToken);

        return reserva.ParaDto();
    }
}
