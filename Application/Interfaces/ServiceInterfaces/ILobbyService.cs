using Application.Model;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface ILobbyService
    {
       public Task<bool> Create(CreateLobbyModel model);
       public Task<bool> JoinLobby();
    }
}
