using Microsoft.AspNetCore.SignalR;

namespace GameLobby.Hubs
{
    public class LobbyHub : Hub
    {
        public static string Group(long lobbyId) => $"lobby:{lobbyId}";

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? ex)
        {
            return base.OnDisconnectedAsync(ex);
        }

        public async Task JoinLobbyGroup(long lobbyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Group(lobbyId));
        }
    }
}
