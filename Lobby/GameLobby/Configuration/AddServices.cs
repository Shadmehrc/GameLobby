using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Services;
using Domain.Configuration;
using Infrastructure.Messaging;
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
            services.AddScoped<ILobbyNotifier, LobbyNotifier>();
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            return services;
        }
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration config)
        {

            services.Configure<RedisOptions>(config.GetSection("Redis"));

            services.Configure<LobbyConfigs>(config.GetSection("LobbyConfigs"));

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var cs = sp.GetRequiredService<IConfiguration>().GetSection("Redis")["ConnectionString"];
                return ConnectionMultiplexer.Connect(cs);
            });

            return services;
        }

    }
}
