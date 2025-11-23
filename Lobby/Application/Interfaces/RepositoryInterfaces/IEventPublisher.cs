using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.RepositoryInterfaces
{
    public interface IEventPublisher
    {
        Task PublishPlayerJoined(string playerId, long lobbyId, DateTime joinedAt);

    }
}
