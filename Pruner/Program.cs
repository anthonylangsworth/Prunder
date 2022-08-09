using CsvHelper;
using Microsoft.Extensions.Configuration;
using Pruner;
using System.Globalization;

IList<DiscordMember> GetDiscordMembers(string discordMembersFile)
{
    using StreamReader streamReader = new(discordMembersFile);
    using CsvReader csvReader = new(streamReader, CultureInfo.InvariantCulture);
    csvReader.Context.RegisterClassMap<DiscordMemberMap>();
    return csvReader.GetRecords<DiscordMember>().ToList();
}

IList<SquadronMember> GetSquadronMembers(string squadronMembersFile)
{
    using StreamReader streamReader = new(squadronMembersFile);
    using CsvReader csvReader = new(streamReader, CultureInfo.InvariantCulture);
    csvReader.Context.RegisterClassMap<SquadronMemberMap>();
    return csvReader.GetRecords<SquadronMember>().ToList();
}

bool SameCommanderName(string discordMember, string squadronMember, IReadOnlyDictionary<string, string> discordToSquadronMemberMap)
{
    return string.Equals(discordMember, squadronMember, StringComparison.InvariantCultureIgnoreCase)
        || string.Equals(discordMember, "CMDR " + squadronMember, StringComparison.InvariantCultureIgnoreCase)
        || (discordToSquadronMemberMap.TryGetValue(discordMember, out string? mappedSquadronMember)
            && string.Equals(mappedSquadronMember, squadronMember, StringComparison.InvariantCultureIgnoreCase));
}

//bool SameLevel(SquadronMember squadronMember, DiscordMember discordMember, Dictionary<string, string[]> rankToRoles)
//{
//    rankToRoles.TryGetValue(squadronMember.RankId.ToString(), out string[]? expectedRoles);
//    return expectedRoles == null || discordMember.Roles.Intersect(expectedRoles).Any();
//}

//int ExpectedRank(SquadronMember squadronMember, DiscordMember discordMember, Dictionary<string, string[]> rankToRoles)
//{
//    rankToRoles.TryGetValue(squadronMember.RankId.ToString(), out string[]? expectedRoles);
//    return expectedRoles == null || discordMember.Roles.Intersect(expectedRoles).Any();
//    rankToRoles.TryGetValue(squadronMember.RankId.ToString(), out string[]? expectedRoles);
//}

IEnumerable<DiscordMember> GetNotInSquadron(
    IEnumerable<SquadronMember> squadronMembers, IEnumerable<DiscordMember> discordMembers, IReadOnlyDictionary<string, string> discordToSquadronMemberMap)
{
    return discordMembers.Where(dm => squadronMembers.All(sm => !SameCommanderName(dm.Name, sm.Name, discordToSquadronMemberMap)));
}

IEnumerable<SquadronMember> GetNotOnDiscord(
    IEnumerable<SquadronMember> squadronMembers, IEnumerable<DiscordMember> discordMembers, IReadOnlyDictionary<string, string> discordToSquadronMemberMap)
{
    return squadronMembers.Where(sm => discordMembers.All(dm => !SameCommanderName(dm.Name, sm.Name, discordToSquadronMemberMap)));
}

IEnumerable<(SquadronMember SquadronMember, DiscordMember DiscordMember)> InBoth(
    IEnumerable<SquadronMember> squadronMembers, IEnumerable<DiscordMember> discordMembers,
    IReadOnlyDictionary<string, string> discordToSquadronMemberMap)
{
    foreach (SquadronMember squadronMember in squadronMembers)
    {
        DiscordMember? matchingDiscordMember = discordMembers.FirstOrDefault(
            dm => SameCommanderName(dm.Name, squadronMember.Name, discordToSquadronMemberMap));
        if (matchingDiscordMember != null)
        {
            yield return (squadronMember, matchingDiscordMember);
        }
    }
}

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Get data
Dictionary<string, string> discordToSquadronMemberMap = new();
config.GetRequiredSection("DiscordToSquadronMemberMap").Bind(discordToSquadronMemberMap);
List<RoleToRank> expectedRoleToRanks = new();
config.GetRequiredSection("RoleToRank").Bind(expectedRoleToRanks);
Dictionary<string, string> rankToDescription = new();
config.GetRequiredSection("RankDescriptions").Bind(rankToDescription);
IList<DiscordMember> discordMembers = GetDiscordMembers(config.GetRequiredSection("DiscordMembersFile").Value);
IList<SquadronMember> squadronMembers = GetSquadronMembers(config.GetRequiredSection("SquadronMembersFile").Value);

// Process the data into something usable
IList<DiscordMember> discordMembersNotInSquadron = GetNotInSquadron(squadronMembers, discordMembers, discordToSquadronMemberMap).ToList();
IList<SquadronMember> squadronMembersNotOnDiscord = GetNotOnDiscord(squadronMembers, discordMembers, discordToSquadronMemberMap).ToList();
IList<(SquadronMember SquadronMember, DiscordMember DiscordMember)> inBoth = InBoth(squadronMembers, discordMembers, discordToSquadronMemberMap).ToList();

//Console.WriteLine("Squadron Memebers Not on the Discord");
//Console.WriteLine(string.Join("\n", squadronMembersNotOnDiscord.Select(sm => sm.Name)));
//Console.WriteLine();

Console.WriteLine("Discord Members Not in the Squadron");
Console.WriteLine(string.Join("\n", discordMembersNotInSquadron.Where(dm => dm.Roles.Contains(DiscordRoles.PCMembers) && !dm.Roles.Contains(DiscordRoles.Veterans))
                                                               .Select(dm => dm.Name)));
Console.WriteLine();

Console.WriteLine("In-game Rank to DiscordRole Mismatch");
Console.WriteLine(string.Join("\n", inBoth.Select(ib =>
                                                {
                                                    RoleToRank? roleToRank = expectedRoleToRanks.FirstOrDefault(r2r => ib.DiscordMember.Roles.Contains(r2r.Role));
                                                    return
                                                    (
                                                        DiscordName: ib.DiscordMember.Name,
                                                        Roles: ib.DiscordMember.Roles,
                                                        SquadronName: ib.SquadronMember.Name,
                                                        CurrentRankId: ib.SquadronMember.RankId,
                                                        ExpectedRankId: roleToRank?.RankId,
                                                        DueToRole: roleToRank?.Role
                                                    );
                                                })
                                          .Where(match => match.ExpectedRankId != null && match.CurrentRankId != match.ExpectedRankId)
                                          .Select(match => $"{match.DiscordName}, {match.SquadronName}, {rankToDescription[match.CurrentRankId.ToString()]}, {rankToDescription[match.ExpectedRankId?.ToString()]}, {match.DueToRole}")));
Console.WriteLine();