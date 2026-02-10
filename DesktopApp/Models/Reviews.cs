using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class Reviews
    {
        [JsonPropertyName("roomId")]
        public string roomId { get; set; }

        public int rating { get; set; }
        public string commet { get; set; }

        public DateTime createdAt { get; set; }
    }
}
