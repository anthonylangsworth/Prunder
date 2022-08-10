namespace Pruner
{
    internal record SquadronMember
    {
        public string Name { get; set; } = "";
        public uint RankId { get; set; }
        public DateTime? DateRequested { get; set; }
        public bool Pending { get; set; }
        public DateTime? DateJoined { get; set; }
        public bool Presence { get; set; }
        public DateTime? DateLastOnline { get; set; }
    }
}
