using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pruner
{
    internal class SquadronMember
    {
        public string Name { get; set; } = "";
        public uint RankId { get; set; }
        public DateTime? Requested { get; set; }
        public bool Pending { get; set; }
        public DateTime? Joined { get; set; }
        public bool Presence { get; set; }
        public DateTime? LastOnline { get; set; }
    }
}
