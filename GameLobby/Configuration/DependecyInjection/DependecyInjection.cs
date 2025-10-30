using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Services;
using Infrastructure.Repositories;

namespace GameLobby.Configuration.DependecyInjection
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILobbyService, LobbyService>();
            services.AddScoped<ILobbyRepository, LobbyRepository>();
            return services;
        }
    }
}
