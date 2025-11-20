using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces
{
    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(string id);
        Task AddAsync(Player player);
    }
}
