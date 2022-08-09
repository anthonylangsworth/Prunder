namespace Pruner
{
    // Map a Discord role name to an expected in-game rank ID.
    internal class RoleToRank
    {
        public string Role { get; set; } = "";
        public int RankId { get; set; }
    }
}
