using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DesktopApp.Models;
using DesktopApp.Services;

namespace DesktopApp.ViewModels
{
    class RoomsViewModel
    {
        public ObservableCollection<Rooms> Rooms { get; set; } = new();
        private readonly ApiClient _api = new ApiClient();
        public RoomsViewModel()
        {
            LoadRoomsAsync();

        }
        private async Task LoadRoomsAsync()
        {
            try
            {
                var list = await _api.GetRooms();

                Rooms.Clear();
                foreach (var r in list)
                    Rooms.Add(r);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
