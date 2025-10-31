using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Model
{
    public class JoinLobbyResult
    {
        public bool Joined { get; init; }
        public LobbyModel Lobby { get; init; } = default!;
        public string Message { get; init; } = string.Empty;

        public static JoinLobbyResult Success(LobbyModel lobby, string message)
            => new() { Joined = true, Lobby = lobby, Message = message };

        public static JoinLobbyResult Fail(LobbyModel lobby, string message)
            => new() { Joined = false, Lobby = lobby, Message = message };
    }
}
