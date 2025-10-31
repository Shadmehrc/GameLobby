using Application.Interfaces;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Model;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{
    public class LobbyService(ILobbyRepository _lobbyRepository, ILobbyNotifier _notifier) : ILobbyService
    {
        public Task<bool> Create(CreateLobbyModel model)
        {
            throw new NotImplementedException();
        }
        public Task<Result<long>> CreateLobby()
        {
            return _lobbyRepository.CreateLobbyAsync();
        }
        public async Task<Result<LobbyModel>> JoinLobby(long lobbyId, string playerId)
        {
            var res = await _lobbyRepository.JoinLobbyAsync(lobbyId, playerId);

            if (res.IsSuccess)
                await _notifier.PlayerJoined(lobbyId, playerId, res.Value.MemberCount);
            else if (res.Code == ErrorCode.Full)
                await _notifier.LobbyFull(lobbyId);
            else if (res.Code == ErrorCode.Locked)
              await _notifier.LobbyFull(lobbyId);

            return res;
        }

        public Task<LobbyModel?> GetLobby(long lobbyId)
        {
            return _lobbyRepository.GetLobbyAsync(lobbyId);
        }
    }

}
