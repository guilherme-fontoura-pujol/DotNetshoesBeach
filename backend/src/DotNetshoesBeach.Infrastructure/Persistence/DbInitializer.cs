using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;
using DotNetshoesBeach.Domain.Enums;
using DotNetshoesBeach.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetshoesBeach.Infrastructure.Persistence;

/// <summary>
/// Executado na subida da API: aplica as migrations, garante a constraint anti-sobreposição de reservas
/// e cria o primeiro admin (se ainda não existir nenhum).
/// </summary>
public static class DbInitializer
{
    // Idempotente: pode rodar a cada inicialização.
    // btree_gist é necessário para combinar "=" (uuid) com "&&" (intervalo) no mesmo índice de exclusão.
    private const string SqlConstraintSobreposicao = """
        CREATE EXTENSION IF NOT EXISTS btree_gist;

        DO $$
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'ex_reservas_sem_sobreposicao') THEN
                ALTER TABLE reservas
                    ADD CONSTRAINT ex_reservas_sem_sobreposicao
                    EXCLUDE USING gist (
                        "QuadraId" WITH =,
                        tstzrange("DataHoraInicio", "DataHoraFim", '[)') WITH &&
                    )
                    WHERE ("Status" = 'Confirmada');
            END IF;
        END
        $$;
        """;

    public static async Task InicializarAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<ApplicationDbContext>();
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        if (!context.Database.GetMigrations().Any())
        {
            throw new InvalidOperationException(
                "Nenhuma migration encontrada. Crie a migration inicial com: " +
                "dotnet ef migrations add InitialCreate " +
                "--project src/DotNetshoesBeach.Infrastructure --startup-project src/DotNetshoesBeach.Api " +
                "--output-dir Persistence/Migrations");
        }

        await context.Database.MigrateAsync(cancellationToken);
        await context.Database.ExecuteSqlRawAsync(SqlConstraintSobreposicao, cancellationToken);

        await CriarAdminInicialAsync(context, provider, logger, cancellationToken);
    }

    private static async Task CriarAdminInicialAsync(
        ApplicationDbContext context, IServiceProvider provider, ILogger logger, CancellationToken cancellationToken)
    {
        if (await context.Clientes.AnyAsync(c => c.Papel == Papel.Admin, cancellationToken))
            return;

        var opcoes = provider.GetRequiredService<IOptions<AdminInicialOptions>>().Value;
        if (string.IsNullOrWhiteSpace(opcoes.Email) || string.IsNullOrWhiteSpace(opcoes.Senha))
        {
            logger.LogWarning(
                "Nenhum administrador existe e a seção 'AdminInicial' (Email/Senha) não está configurada. " +
                "Ninguém conseguirá usar as rotas de admin.");
            return;
        }

        var hasher = provider.GetRequiredService<IPasswordHasher>();
        var admin = new Cliente(opcoes.Nome, opcoes.Email, string.Empty, hasher.HashPassword(opcoes.Senha), Papel.Admin);

        context.Clientes.Add(admin);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Administrador inicial criado: {Email}", admin.Email);
    }
}
