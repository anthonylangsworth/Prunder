using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruner
{
    internal class SquadronMemberMap: ClassMap<SquadronMember>
    {
        public SquadronMemberMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.RankId).Name("Squadron Rank");
            Map(m => m.DateRequested).Name("Date Requested").Optional();
            Map(m => m.Pending).Name("Pending");
            Map(m => m.DateJoined).Name("Date Joined").Optional();
            Map(m => m.Presence).Name("Online");
            Map(m => m.DateLastOnline).Name("Last Online").Optional();
        }
    }
}
