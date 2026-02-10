using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class User
    {
        public string Id { get; set; }   // ObjectId de Mongo
        public string Name { get; set; } // Nombre completo
    }

}
