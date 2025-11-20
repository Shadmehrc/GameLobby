using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly Dictionary<string, Player> _players = new();

        public Task<Player?> GetByIdAsync(string id)
        {
            _players.TryGetValue(id, out var player);
            return Task.FromResult(player);
        }

        public Task AddAsync(Player player)
        {
            _players[player.Id] = player;
            return Task.CompletedTask;
        }
    }
}
