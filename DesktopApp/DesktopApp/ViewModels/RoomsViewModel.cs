using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.Models;

namespace DesktopApp.ViewModels
{
    class RoomsViewModel
    {
        public ObservableCollection<Rooms> Rooms { get; set; } = new();

        public RoomsViewModel()
        {
            // de momento vacío
        }
    }
}
