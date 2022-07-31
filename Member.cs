using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SquadronDump
{
    internal class Member
    {
        [JsonPropertyName("squadron_id")]
        public uint SquadronId { get; set; }

        [JsonPropertyName("member_id")]
        public uint MemberId { get; set; }

        [JsonPropertyName("joined")]
        public string? Joined { get; set; }

        [JsonPropertyName("joined_ts")]
        public long JoinedTs { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("applied")]
        public bool Applied { get; set; }

        [JsonPropertyName("requested")]
        public string? Requested { get; set; }

        [JsonPropertyName("requested_ts")]
        public ulong RequestedTs { get; set; }

        [JsonPropertyName("rank_id")]
        public uint RankId { get; set; }

        [JsonPropertyName("request_letter")]
        public string? RequestLetter { get; set; }

        [JsonPropertyName("credit_contributions")]
        public ulong CreditContributions { get; set; }

        [JsonPropertyName("presence")]
        public bool Presence { get; set; }

        [JsonPropertyName("user_id")]
        public uint UserId { get; set; }

        [JsonPropertyName("rankCombat")]
        public uint RankCombat { get; set; }

        [JsonPropertyName("rankExplore")]
        public uint RankExplore { get; set; }

        [JsonPropertyName("rankTrade")]
        public uint RankTrade { get; set; }

        [JsonPropertyName("rankSoldier")]
        public uint RankSoldier { get; set; }

        [JsonPropertyName("rankExobiologist")]
        public uint RankExobiologist { get; set; }

        [JsonPropertyName("lastOnline")]
        public string? LastOnline { get; set; }

        [JsonPropertyName("lastOnline_ts")]
        public ulong LastOnlineTs { get; set; }

        [JsonPropertyName("name")]
        [JsonConverter(typeof(NameJsonConverter))]
        public string? Name { get; set; }
    }
}
