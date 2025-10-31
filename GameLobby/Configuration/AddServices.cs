using Application.Interfaces;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Services;
using Domain.Configuration;
using Infrastructure.Repositories;
using StackExchange.Redis;

namespace GameLobby.Configuration
{
    public static class AddServices
    {
        public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
        {
            services.AddScoped<ILobbyService, LobbyService>();
            services.AddScoped<ILobbyRepository, LobbyRepository>();
            services.AddScoped<ILobbyNotifier, SignalRLobbyNotifier>();
            return services;
        }
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration config)
        {
            // اتصال Redis یکبار (Singleton)
            services.Configure<RedisOptions>(config.GetSection("Redis"));

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var cs = sp.GetRequiredService<IConfiguration>().GetSection("Redis")["ConnectionString"];
                return ConnectionMultiplexer.Connect(cs);
            });

            return services;
        }

    }
}
