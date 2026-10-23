using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetshoesBeach.Infrastructure.Persistence.Repositories;

public class QuadraRepository : IQuadraRepository
{
    private readonly ApplicationDbContext _context;

    public QuadraRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Quadra?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Quadras.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<bool> ExisteNomeAsync(string nome, Guid? ignorarId = null, CancellationToken cancellationToken = default)
    {
        var nomeNormalizado = nome.Trim().ToLower();
        return await _context.Quadras
            .AnyAsync(q => q.Nome.ToLower() == nomeNormalizado && (ignorarId == null || q.Id != ignorarId), cancellationToken);
    }

    public async Task<IReadOnlyList<Quadra>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Quadras
            .AsNoTracking()
            .OrderBy(q => q.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Quadra quadra, CancellationToken cancellationToken = default)
    {
        await _context.Quadras.AddAsync(quadra, cancellationToken);
    }

    public void Remover(Quadra quadra)
    {
        _context.Quadras.Remove(quadra);
    }
}
