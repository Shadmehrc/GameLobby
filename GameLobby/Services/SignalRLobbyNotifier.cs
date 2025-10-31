using Application.Interfaces;
using GameLobby.Hubs;
using Microsoft.AspNetCore.SignalR;

public class SignalRLobbyNotifier : ILobbyNotifier
{
    private readonly IHubContext<LobbyHub> _hub;
    private readonly ILogger<SignalRLobbyNotifier> _logger;

    public SignalRLobbyNotifier(IHubContext<LobbyHub> hub)
    { _hub = hub;}

    public Task PlayerJoined(long lobbyId, string playerId, int memberCount)
    {
        // ✅ به‌جای Group(...) به همه بفرست
        return _hub.Clients.All
            .SendAsync("PlayerJoined", new { lobbyId, playerId, memberCount });
    }

    public Task LobbyFull(long lobbyId)
    {
        // ✅ به همه بفرست
        return _hub.Clients.All
            .SendAsync("LobbyFull", new { lobbyId });
    }
}
