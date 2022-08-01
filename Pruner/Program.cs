using CsvHelper;
using System.Globalization;
using Pruner;
using Microsoft.Extensions.Configuration;

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

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Dictionary<string, string> discordToSquadronMemberMap = new();
config.GetRequiredSection("DiscordToSquadronMemberMap").Bind(discordToSquadronMemberMap);

IList<DiscordMember> discordMembers = GetDiscordMembers(config.GetRequiredSection("DiscordMembersFile").Value);
IList<SquadronMember> squadronMembers = GetSquadronMembers(config.GetRequiredSection("SquadronMembersFile").Value);

foreach(string name in squadronMembers.Where(sm => discordMembers.All(dm => !SameCommanderName(dm.Name, sm.Name, discordToSquadronMemberMap))).Select(sm => sm.Name))
{
    Console.WriteLine(name);
}