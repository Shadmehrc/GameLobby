using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messaging
{
    public class PlayerEventsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string ExchangeName = "game.events";
        private const string QueueName = "players.player-joined";

        public PlayerEventsConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "admin"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(QueueName, ExchangeName, routingKey: "player.joined");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var msg = JsonSerializer.Deserialize<PlayerJoinedMessage>(json);

                if (msg is null)
                {
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();

                await playerService.SetStatusInLobbyAsync(msg.PlayerId, msg.LobbyId);

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }

        private sealed class PlayerJoinedMessage
        {
            public string PlayerId { get; set; } = default!;
            public long LobbyId { get; set; }
            public DateTime JoinedAt { get; set; }
        }
    }
}
