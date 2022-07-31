using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruner
{
    internal class DiscordMember
    {
        public string Name { get; set; } = "";
        public IReadOnlySet<string> Roles { get; set; } = new HashSet<string>();
    }
}
