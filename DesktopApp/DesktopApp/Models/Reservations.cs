using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class Reservations
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("userId")]
        public string User { get; set; }           
        public List<Rooms> Rooms { get; set; } = new List<Rooms>();

        [JsonPropertyName("roomIds")]
        public List<string> RoomIds { get; set; } = new List<string>();

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public string Status { get; set; }

        public string RoomNumbers
        {
            get
            {
                return Rooms != null && Rooms.Any()
                    ? string.Join(", ", Rooms.Select(r => r.numRoom))
                    : "";
            }
        }

    }
}
