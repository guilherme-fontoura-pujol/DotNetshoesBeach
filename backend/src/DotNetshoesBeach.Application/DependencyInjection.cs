using DotNetshoesBeach.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetshoesBeach.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClienteService, ClienteService>();
        return services;
    }
}