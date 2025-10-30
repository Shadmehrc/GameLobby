
using Application.Interfaces.RepositoryInterfaces;
using Application.Model;

namespace Infrastructure.Repositories
{
    public class LobbyRepository : ILobbyRepository
    {
        public Task<bool> Create(CreateLobbyModel model)
        {
            throw new NotImplementedException();
        }
    }
}
