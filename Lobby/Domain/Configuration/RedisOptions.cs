using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configuration
{
    public class RedisOptions
    {
        public string? ConnectionString { get; set; }
        public string InstancePrefix { get; set; } = "lobby";
    }
}
