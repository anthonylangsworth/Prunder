namespace Pruner
{
    // Map a Discord role name to an expected in-game rank ID.
    internal record RoleToRank
    {
        public string Role { get; set; } = "";
        public uint RankId { get; set; }
    }
}
