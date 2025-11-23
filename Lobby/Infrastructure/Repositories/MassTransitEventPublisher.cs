using Application.Interfaces.RepositoryInterfaces;
using MassTransit;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging
{
    public record PlayerJoined
    {
        public string PlayerId { get; init; } = default!;
        public long LobbyId { get; init; }
        public DateTime JoinedAt { get; init; }
    }
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishPlayerJoined(string playerId, long lobbyId, DateTime joinedAt)
        {
            await _publishEndpoint.Publish(new PlayerJoined
            {
                PlayerId = playerId,
                LobbyId = lobbyId,
                JoinedAt = joinedAt
            });
        }
    }
}

