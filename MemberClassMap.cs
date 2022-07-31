using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadronDump
{
    internal class MemberClassMap: ClassMap<Member>
    {
        public MemberClassMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.RankId).Name("Squadron Rank");
            Map(m => m.Requested).Name("Date Requested");
            Map(m => m.Pending).Name("Pending");
            Map(m => m.Joined).Name("Date Joined");
            Map(m => m.Presence).Name("Online");
            Map(m => m.LastOnline).Name("Last Online"); ;
        }
    }
}
