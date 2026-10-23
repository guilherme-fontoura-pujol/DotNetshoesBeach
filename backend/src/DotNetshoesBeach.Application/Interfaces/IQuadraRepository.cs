using DotNetshoesBeach.Domain.Entities;

namespace DotNetshoesBeach.Application.Interfaces;

public interface IQuadraRepository
{
    Task<Quadra?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExisteNomeAsync(string nome, Guid? ignorarId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Quadra>> ListarAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Quadra quadra, CancellationToken cancellationToken = default);
    void Remover(Quadra quadra);
}
