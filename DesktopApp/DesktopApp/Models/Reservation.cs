using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class Reservation
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string RoomId { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public string Status { get; set; }

    }
}
