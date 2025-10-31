namespace Application.Interfaces
{
    public interface ILobbyNotifier
    {
        Task PlayerJoined(long lobbyId, string playerId, int memberCount);
        Task LobbyFull(long lobbyId);
        Task LobbyError(long lobbyId);
        public static string Group(long lobbyId) => $"lobby:{lobbyId}";
    }
}
