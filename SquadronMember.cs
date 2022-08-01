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
        public DateTime? DateRequested { get; set; }
        public bool Pending { get; set; }
        public DateTime? DateJoined { get; set; }
        public bool Presence { get; set; }
        public DateTime? DateLastOnline { get; set; }
    }
}
