using System.Data;
using curs.Models;
using System.Text.Json.Serialization;

namespace curs.Models
{
    public class Bonus
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("role_id")]
        public ulong RoleId { get; set; }

        [JsonPropertyName("role")]
        public Role? Role { get; set; }
    }
}