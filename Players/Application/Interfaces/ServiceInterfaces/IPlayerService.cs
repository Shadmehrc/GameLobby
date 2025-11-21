using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IPlayerService
    {
        Task<Player?> GetByIdAsync(string id);
        Task<Player> CreateAsync(string playerId, string userName);
        Task SetStatusInLobbyAsync(string playerId, long lobbyId);
    }
}

