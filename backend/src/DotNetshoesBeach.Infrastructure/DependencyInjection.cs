using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Infrastructure.Options;
using DotNetshoesBeach.Infrastructure.Persistence;
using DotNetshoesBeach.Infrastructure.Persistence.Repositories;
using DotNetshoesBeach.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetshoesBeach.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' não encontrada. Veja docs/database/README.md.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.TryAddSingleton(TimeProvider.System);

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Secao));
        services.Configure<AdminInicialOptions>(configuration.GetSection(AdminInicialOptions.Secao));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IQuadraRepository, QuadraRepository>();
        services.AddScoped<IReservaRepository, ReservaRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
