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
        public enum roomType { Single, Double, Triple, Fourfold}
        public string description { get; set; }

        public List<string> image { get; set; }

        public float pricePerNight { get; set; }
        public int maxOccupancy{ get; set; }

        public List<string> reviews { get; set; }
        public  enum availability {  Available, Unavailable,Block }
    }
}
