using Application.Model;
using Domain.Entities;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface ILobbyService
    {
        Task<Result<long>> CreateLobby();
        Task<Result<Lobby>> JoinLobby(long lobbyId, string playerId);
        Task<Lobby?> GetLobby(long lobbyId);
    }
}
