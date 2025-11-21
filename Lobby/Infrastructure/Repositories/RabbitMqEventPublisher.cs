using System.Text;
using System.Text.Json;
using Application.Interfaces.RepositoryInterfaces;
using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "game.events";

        public RabbitMqEventPublisher()
        {
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
        }

        public void PublishPlayerJoined(string playerId, long lobbyId, DateTime joinedAt)
        {
            var payload = new
            {
                PlayerId = playerId,
                LobbyId = lobbyId,
                JoinedAt = joinedAt
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: "player.joined",
                basicProperties: null,
                body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
