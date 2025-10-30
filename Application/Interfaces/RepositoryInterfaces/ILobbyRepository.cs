using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Model;

namespace Application.Interfaces.RepositoryInterfaces
{
    public interface ILobbyRepository
    {
        public Task<bool> Create(CreateLobbyModel model);
    }
}
