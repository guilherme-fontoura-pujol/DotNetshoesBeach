using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Infrastructure.Persistence;
using DotNetshoesBeach.Infrastructure.Persistence.Repositories;
using DotNetshoesBeach.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetshoesBeach.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}