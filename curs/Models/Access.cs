using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace curs.Models
{
    public class Access
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("startChange")]
        public TimeSpan StartChange { get; set; }

        [JsonPropertyName("endChange")]
        public TimeSpan EndChange { get; set; }

        [JsonPropertyName("confirm")]
        public int Confirm { get; set; }

        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
    }
}
