using Application.Interfaces;
using GameLobby.Hubs;
using Microsoft.AspNetCore.SignalR;

public class LobbyNotifier(IHubContext<LobbyHub> _hub) : ILobbyNotifier
{
    public Task PlayerJoined(long lobbyId, string playerId, int memberCount)
    {
        return _hub.Clients.All
            .SendAsync("PlayerJoined", new { lobbyId, playerId, memberCount });
    }

    public Task LobbyFull(long lobbyId)
    {
        return _hub.Clients.All
            .SendAsync("LobbyFull", new { lobbyId });
    }

    public Task LobbyError(long lobbyId)
    {
        return _hub.Clients.All
            .SendAsync("LobbyError", new { lobbyId });
    }
}
