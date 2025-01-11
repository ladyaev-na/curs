using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace curs.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("user")]
        public required User User { get; set; }
        [JsonPropertyName("token")]
        public required string Token { get; set; }
    }
}
