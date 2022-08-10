namespace Pruner.Attic
{
    internal class DiscordRoleToRankMismatchReport : Report
    {
        public DiscordRoleToRankMismatchReport(List<RoleToRank> expectedRoleToRanks, Dictionary<string, string> rankToDescription, IList<(SquadronMember SquadronMember, DiscordMember DiscordMember)> inBoth)
        {
            ExpectedRoleToRanks = expectedRoleToRanks;
            RankToDescription = rankToDescription;
            InBoth = inBoth;
        }

        public List<RoleToRank> ExpectedRoleToRanks { get; }
        public Dictionary<string, string> RankToDescription { get; }
        public IList<(SquadronMember SquadronMember, DiscordMember DiscordMember)> InBoth { get; }

        public override void Generate(TextWriter textWriter)
        {
            throw new NotImplementedException();
        }
    }
}
