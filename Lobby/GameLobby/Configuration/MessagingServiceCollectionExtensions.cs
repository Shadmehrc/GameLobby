namespace GameLobby.Configuration;
using Infrastructure.Messaging;
using Application.Interfaces.RepositoryInterfaces;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // اگه کانسومر داشتی اینجا اضافه می‌کنی
            // x.AddConsumer<PlayerJoinedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // معادل ExchangeDeclare("game.events", "topic")
                cfg.Message<PlayerJoined>(m =>
                {
                    m.SetEntityName("game.events"); // اسم exchange
                });

                cfg.Publish<PlayerJoined>(p =>
                {
                    p.ExchangeType = "direct"; // نوع exchange
                });

                // اگه کانسومر هم داری، اینجا ReceiveEndpoint می‌ذاری
                // cfg.ReceiveEndpoint("game-lobby-service", e =>
                // {
                //     e.ConfigureConsumer<PlayerJoinedConsumer>(context);
                //     e.Bind("game.events", b =>
                //     {
                //         b.RoutingKey = "player.joined";
                //         b.ExchangeType = "topic";
                //     });
                // });
            });
        });

        services.AddMassTransitHostedService(true);

      
        return services;
    }
}
