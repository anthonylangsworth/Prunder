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
    .AddCommandLine(Environment.GetCommandLineArgs())
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

// TODO: Allow report selection via configuration e.g. command line
ReportType report = ReportType.DiscordRoleToInGameSquadronMismatch;
try
{
    // report = Enum.Parse<ReportType>(config.GetRequiredSection("Report").Value);
}
catch (InvalidOperationException)
{
    Console.Error.WriteLine("Must specify the report");
    Environment.Exit(1);
}


switch (report)
{
    case ReportType.SquadronMembersNotOnDiscord:
        SquadronMembersNotOnDiscord(squadronMembersNotOnDiscord, Console.Out);
        break;
    case ReportType.DiscordMembersNotInSquadron:
        DiscordMembersNotInSquadron(discordMembersNotInSquadron, Console.Out);
        break;
    case ReportType.DiscordRoleToInGameSquadronMismatch:
        DiscordRoleToRankMismatch(expectedRoleToRanks, rankToDescription, inBoth, Console.Out);
        break;
    default:
        Console.Error.WriteLine("Unsupported or unknown report");
        Environment.Exit(1);
        break;
}

// List the PC squadron members not on Discord. They should be encouraged to join.
void SquadronMembersNotOnDiscord(IList<SquadronMember> squadronMembersNotOnDiscord, TextWriter textWriter)
{
    using CsvWriter csvWriter = new(textWriter, CultureInfo.InvariantCulture);
    csvWriter.WriteRecords(squadronMembersNotOnDiscord.OrderBy(match => match.Name)
                                                      .Select(dm => new { dm.Name }));
}

// List the discord PC members not in the squadron. They should be removed from PC Members and added to Mavericks.
void DiscordMembersNotInSquadron(IList<DiscordMember> discordMembersNotInSquadron, TextWriter textWriter)
{
    using CsvWriter csvWriter = new(textWriter, CultureInfo.InvariantCulture);
    csvWriter.WriteRecords(discordMembersNotInSquadron.Where(dm => dm.Roles.Contains(DiscordRoles.PCMembers) && !dm.Roles.Contains(DiscordRoles.Veterans))
                                                      .OrderBy(match => match.Name)
                                                      .Select(dm => new { dm.Name }));
}

// List the squadron members whose in-game rank does not match their Discord role. Their in-game rank should be corrected.
void DiscordRoleToRankMismatch(List<RoleToRank> expectedRoleToRanks, Dictionary<string, string> rankToDescription, IList<(SquadronMember SquadronMember, DiscordMember DiscordMember)> inBoth, TextWriter textWriter)
{
    using CsvWriter csvWriter = new(textWriter, CultureInfo.InvariantCulture);
    csvWriter.Context.RegisterClassMap(new DiscordRoleToRankMismatchMap(rankToDescription));
    csvWriter.WriteRecords(
        inBoth.Select(ib =>
        {
            RoleToRank? roleToRank = expectedRoleToRanks.FirstOrDefault(r2r => ib.DiscordMember.Roles.Contains(r2r.Role));
            return new DiscordRoleToRankMismatch()
            {
                DiscordName = ib.DiscordMember.Name,
                Roles = ib.DiscordMember.Roles,
                SquadronName = ib.SquadronMember.Name,
                CurrentRankId = ib.SquadronMember.RankId,
                ExpectedRankId = roleToRank?.RankId,
                DueToRole = roleToRank?.Role
            };
        })
        .Where(match => match.ExpectedRankId != null && match.CurrentRankId != match.ExpectedRankId)
        .OrderBy(match => match.DiscordName));
}
