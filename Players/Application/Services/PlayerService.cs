using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;

namespace Application.Services
{

    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }


        public async Task<Player> CreateAsync(string playerId, string userName)
        {
            var existing = await _playerRepository.GetByIdAsync(playerId);
            if (existing is not null)
                throw new InvalidOperationException($"Player with id '{playerId}' already exists.");

            var player = new Player()
            {
                Id=playerId,
                UserName=userName
            };

            await _playerRepository.AddAsync(player);

            return player;
        }

        public async Task<Player?> GetByIdAsync(string id)
        {

            return await _playerRepository.GetByIdAsync(id);
   
        }

        public async  Task SetStatusInLobbyAsync(string playerId, long lobbyId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player is null)
            {
                Console.WriteLine("magiccc");
                return;
            }
            player.Status = PlayerStatus.InLobby;
        }
    }
}
