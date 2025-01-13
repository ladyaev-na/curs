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
        public string Price { get; set; }

        [JsonPropertyName("role_id")]
        public ulong RoleId { get; set; }

        [JsonPropertyName("role")]
        public Role? Role { get; set; }
        public string FormattedPrice
        {
            get
            {
                if (string.IsNullOrEmpty(Price)) return "0 ₽";
                // Разделяем рубли и копейки
                var parts = Price.Split('.'); var rubles = parts[0];
                var kopecks = parts.Length > 1 ? parts[1] : "00";
                // Если копейки "00", отображаем только рубли
                if (kopecks == "00") 
                    return $"{rubles} ₽";
                // Иначе отображаем рубли и копейки
                return $"{rubles}.{kopecks} ₽";
            }
        }
    }
}