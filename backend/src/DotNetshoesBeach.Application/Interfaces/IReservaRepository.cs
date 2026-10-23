using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Domain.Entities;
using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Application.Interfaces;

public interface IReservaRepository
{
    /// <summary>Carrega a reserva já com Cliente e Quadra.</summary>
    Task<Reserva?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Listagem para telas: filtra, ordena, pagina e projeta direto para DTO no banco
    /// (só as colunas necessárias, sem rastreamento de entidades).
    /// </summary>
    Task<PaginaDto<ReservaResponseDto>> ListarPaginadoAsync(
        Guid? quadraId, Guid? clienteId, DateTime? deUtc, DateTime? ateUtc, StatusReserva? status,
        int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReservaResponseDto>> ListarDoClienteAsync(
        Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>Reservas confirmadas, agrupadas por quadra, com quantidade e valores no período [deUtc, ateUtc).</summary>
    Task<IReadOnlyList<ResumoQuadraDto>> ResumirPorQuadraAsync(
        DateTime deUtc, DateTime ateUtc, CancellationToken cancellationToken = default);

    Task<int> ContarAtivasDaQuadraAsync(
        Guid quadraId, DateTime agoraUtc, CancellationToken cancellationToken = default);

    /// <summary>Reservas confirmadas que se sobrepõem ao intervalo [deUtc, ateUtc) em uma quadra.</summary>
    Task<IReadOnlyList<Reserva>> ListarConfirmadasNoPeriodoAsync(
        Guid quadraId, DateTime deUtc, DateTime ateUtc, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoAsync(
        Guid quadraId, DateTime inicioUtc, DateTime fimUtc, CancellationToken cancellationToken = default);

    /// <summary>Reservas confirmadas que ainda não terminaram (em andamento ou futuras) de uma quadra.</summary>
    Task<IReadOnlyList<Reserva>> ListarAtivasDaQuadraAsync(
        Guid quadraId, DateTime agoraUtc, CancellationToken cancellationToken = default);

    Task<bool> ClienteTemReservasAtivasAsync(
        Guid clienteId, DateTime agoraUtc, CancellationToken cancellationToken = default);

    Task AdicionarAsync(Reserva reserva, CancellationToken cancellationToken = default);
}
