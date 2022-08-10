namespace Pruner
{
    internal record DiscordMember
    {
        public string Name { get; set; } = "";
        public IReadOnlySet<string> Roles { get; set; } = new HashSet<string>();
    }
}
