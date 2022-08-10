using CsvHelper.Configuration;

namespace Pruner
{
    internal class DiscordRoleToRankMismatchMap : ClassMap<DiscordRoleToRankMismatch>
    {
        public DiscordRoleToRankMismatchMap(IReadOnlyDictionary<string, string> rankToDescription)
        {
            Map(m => m.DiscordName).Name("Discord Name");
            Map(m => m.Roles).Ignore(); // .Name("Roles").Convert(args => string.Join(", ", args.Value.Roles));
            Map(m => m.SquadronName).Name("Squadron Name");
            Map(m => m.CurrentRankId).Name("Current Rank Id").Convert(args => rankToDescription[args.Value.CurrentRankId.ToString()]);
            Map(m => m.ExpectedRankId).Name("Expected Rank Id").Optional().Convert(args => rankToDescription[args.Value.ExpectedRankId?.ToString() ?? ""]);
            Map(m => m.DueToRole).Name("Due To Role").Optional();
        }
    }
}
