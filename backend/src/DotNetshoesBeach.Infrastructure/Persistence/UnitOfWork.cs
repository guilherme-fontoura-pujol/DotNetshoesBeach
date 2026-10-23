using DotNetshoesBeach.Application.Exceptions;
using DotNetshoesBeach.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DotNetshoesBeach.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
        {
            switch (pg.SqlState)
            {
                case PostgresErrorCodes.ExclusionViolation:
                    throw new ConflitoException("Já existe uma reserva para esta quadra nesse horário.");
                case PostgresErrorCodes.UniqueViolation:
                    throw new ConflitoException("Já existe um registro com esses dados (valor duplicado).");
                case PostgresErrorCodes.ForeignKeyViolation:
                    throw new RegraDeNegocioException("Operação inválida: há registros relacionados.");
                default:
                    throw;
            }
        }
    }
}
