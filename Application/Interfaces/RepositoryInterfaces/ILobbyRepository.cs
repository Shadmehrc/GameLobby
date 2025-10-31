using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Model;
using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces
{
    public interface ILobbyRepository
    {
        Task<Result<long>> CreateLobbyAsync();
        Task<Result<LobbyModel>> JoinLobbyAsync(long lobbyId, string playerId);
        Task<LobbyModel?> GetLobbyAsync(long lobbyId);
    }
}
