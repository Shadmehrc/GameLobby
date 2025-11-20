using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Services;
using Infrastructure.Repositories;

namespace Players.Configuration
{
    public static class AddServices
    {
        public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
        {
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddSingleton<IPlayerRepository, PlayerRepository>();
            return services;
        }
    }
}
