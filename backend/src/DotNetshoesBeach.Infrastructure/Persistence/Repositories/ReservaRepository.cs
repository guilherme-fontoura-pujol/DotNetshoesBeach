using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;
using DotNetshoesBeach.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotNetshoesBeach.Infrastructure.Persistence.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly ApplicationDbContext _context;

    public ReservaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Reserva?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .Include(r => r.Cliente)
            .Include(r => r.Quadra)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<PaginaDto<ReservaResponseDto>> ListarPaginadoAsync(
        Guid? quadraId, Guid? clienteId, DateTime? deUtc, DateTime? ateUtc, StatusReserva? status,
        int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        // Os filtros são aplicados ANTES de executar a consulta: o Where roda no banco (deferred execution).
        var query = _context.Reservas.AsNoTracking().AsQueryable();

        if (quadraId.HasValue) query = query.Where(r => r.QuadraId == quadraId.Value);
        if (clienteId.HasValue) query = query.Where(r => r.ClienteId == clienteId.Value);
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);

        // Reservas que se sobrepõem ao período informado.
        if (deUtc.HasValue) query = query.Where(r => r.DataHoraFim > deUtc.Value);
        if (ateUtc.HasValue) query = query.Where(r => r.DataHoraInicio < ateUtc.Value);

        var total = await query.CountAsync(cancellationToken);

        var itens = await query
            // Ordenação única (Id desempata) para a paginação ser previsível.
            .OrderByDescending(r => r.DataHoraInicio)
            .ThenBy(r => r.Id)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(r => new ReservaResponseDto(
                r.Id, r.ClienteId, r.Cliente.Nome, r.QuadraId, r.Quadra.Nome,
                r.DataHoraInicio, r.DataHoraFim, r.Valor, r.Status))
            .ToListAsync(cancellationToken);

        return new PaginaDto<ReservaResponseDto>(itens, pagina, tamanhoPagina, total);
    }

    public async Task<IReadOnlyList<ReservaResponseDto>> ListarDoClienteAsync(
        Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .AsNoTracking()
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.DataHoraInicio)
            .ThenBy(r => r.Id)
            .Select(r => new ReservaResponseDto(
                r.Id, r.ClienteId, r.Cliente.Nome, r.QuadraId, r.Quadra.Nome,
                r.DataHoraInicio, r.DataHoraFim, r.Valor, r.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ResumoQuadraDto>> ResumirPorQuadraAsync(
        DateTime deUtc, DateTime ateUtc, CancellationToken cancellationToken = default)
    {
        // Intervalo [de, ate): inclui o instante inicial e exclui o final.
        // Vira um único SELECT ... JOIN ... GROUP BY no PostgreSQL.
        return await _context.Reservas
            .AsNoTracking()
            .Where(r => r.Status == StatusReserva.Confirmada
                        && r.DataHoraInicio >= deUtc
                        && r.DataHoraInicio < ateUtc)
            .GroupBy(r => new { r.QuadraId, r.Quadra.Nome })
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key.Nome)
            .Select(g => new ResumoQuadraDto(
                g.Key.QuadraId,
                g.Key.Nome,
                g.Count(),
                g.Sum(r => r.Valor),
                g.Average(r => r.Valor)))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ContarAtivasDaQuadraAsync(
        Guid quadraId, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .CountAsync(r => r.QuadraId == quadraId
                             && r.Status == StatusReserva.Confirmada
                             && r.DataHoraFim > agoraUtc, cancellationToken);
    }

    public async Task<IReadOnlyList<Reserva>> ListarConfirmadasNoPeriodoAsync(
        Guid quadraId, DateTime deUtc, DateTime ateUtc, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .AsNoTracking()
            .Where(r => r.QuadraId == quadraId
                        && r.Status == StatusReserva.Confirmada
                        && r.DataHoraInicio < ateUtc
                        && r.DataHoraFim > deUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteConflitoAsync(
        Guid quadraId, DateTime inicioUtc, DateTime fimUtc, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .AnyAsync(r => r.QuadraId == quadraId
                           && r.Status == StatusReserva.Confirmada
                           && r.DataHoraInicio < fimUtc
                           && r.DataHoraFim > inicioUtc, cancellationToken);
    }

    public async Task<IReadOnlyList<Reserva>> ListarAtivasDaQuadraAsync(
        Guid quadraId, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .Where(r => r.QuadraId == quadraId
                        && r.Status == StatusReserva.Confirmada
                        && r.DataHoraFim > agoraUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ClienteTemReservasAtivasAsync(
        Guid clienteId, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .AnyAsync(r => r.ClienteId == clienteId
                           && r.Status == StatusReserva.Confirmada
                           && r.DataHoraFim > agoraUtc, cancellationToken);
    }

    public async Task AdicionarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        await _context.Reservas.AddAsync(reserva, cancellationToken);
    }
}
