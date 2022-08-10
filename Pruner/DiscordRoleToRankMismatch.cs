namespace Pruner
{
    internal record DiscordRoleToRankMismatch
    {
        public string DiscordName { get; set; } = "";
        public IReadOnlySet<string> Roles { get; set; } = new HashSet<string>();
        public string SquadronName { get; set; } = "";
        public uint CurrentRankId { get; set; }
        public uint? ExpectedRankId { get; set; }
        public string? DueToRole { get; set; }
    }
}
