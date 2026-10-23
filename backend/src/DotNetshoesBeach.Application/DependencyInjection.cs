using DotNetshoesBeach.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetshoesBeach.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IQuadraService, QuadraService>();
        services.AddScoped<IReservaService, ReservaService>();
        services.AddScoped<IRelatorioService, RelatorioService>();
        return services;
    }
}
