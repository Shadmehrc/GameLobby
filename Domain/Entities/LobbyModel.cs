using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum LobbyStatus
    {
        Open = 0,
        Full = 1,
        Closed = 2
    }

    public class LobbyModel
    {
        public long Id { get; set; }
        public int Capacity { get; set; } = 64;
        public LobbyStatus Status { get; set; } = LobbyStatus.Open;
        public bool ClusterLocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int MemberCount { get; set; } = 0;
    }
}
