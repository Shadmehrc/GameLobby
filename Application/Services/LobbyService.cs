using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Model;

namespace Application.Services
{
    public class LobbyService(ILobbyRepository lobbyRepository) : ILobbyService
    {
        public Task<bool> Create(CreateLobbyModel model)
        {

            throw new NotImplementedException();
        }

        public Task<bool> JoinLobby()
        {
            return Task.FromResult(true);
        }
    }
}
