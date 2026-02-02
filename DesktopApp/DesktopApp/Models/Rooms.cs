using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class Rooms
    {
        public int numRoom { get; set; }
        public int roomFloor { get; set; }
        public string roomType { get; set; }
        public string description { get; set; }

        public List<string> image { get; set; }

        public float pricePerNight { get; set; }
        public int maxOccupancy{ get; set; }

        public List<string> reviews { get; set; }
        public string availability {  get; set; }
    }
}
