using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum PlayerStatus
    {
        Offline = 0,
        InLobby = 1,
        InGame = 2
    }

    public class Player
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public PlayerStatus Status { get; set; } = PlayerStatus.Offline;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
            
    }
}
