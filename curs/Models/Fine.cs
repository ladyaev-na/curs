using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace curs.Models
{
    public class Fine
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
