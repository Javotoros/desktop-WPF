using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class Rooms
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        public int numRoom { get; set; }
        public int numFloor { get; set; }
        public enum RoomType { Single, Double, Triple, Fourfold}
        public RoomType roomType { get; set; }
        public string description { get; set; }

        public List<string> image { get; set; }

        public float pricePerNight { get; set; }
        public int maxOccupancy{ get; set; }

        public List<string> reviews { get; set; }
        public  enum Availability {  Available, Unavailable,Block }
        public Availability availability { get; set; }
    }
}
