﻿using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruner
{
    internal class DiscordMemberMap : ClassMap<DiscordMember>
    {
        public DiscordMemberMap()
        {
            Map(m => m.Name).Name("User");
            Map(m => m.Roles).Convert(GetRoles);
        }

        private IReadOnlySet<string> GetRoles(ConvertFromStringArgs args)
        {
            HashSet<string> result = new HashSet<string>();
            for(int i = 1; i < args.Row.HeaderRecord.Length; i++)
            {
                if (args.Row.GetField(i) == "Y")
                {
                    result.Add(args.Row.HeaderRecord[i] ?? "");
                }
            }
            return result;
        }
    }
}