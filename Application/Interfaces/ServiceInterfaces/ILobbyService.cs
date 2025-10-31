using Application.Model;
using Domain.Entities;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface ILobbyService
    {
        Task<Result<long>> CreateLobby();
        Task<Result<LobbyModel>> JoinLobby(long lobbyId, string playerId);
        Task<LobbyModel?> GetLobby(long lobbyId);
    }
}
