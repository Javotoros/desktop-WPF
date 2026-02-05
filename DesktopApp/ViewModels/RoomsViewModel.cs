using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DesktopApp.Models;
using DesktopApp.Services;

namespace DesktopApp.ViewModels
{
    public class RoomsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Rooms> Rooms { get; set; } = new();
        private readonly ApiClient _api = new ApiClient();
        public Array RoomTypes => Enum.GetValues(typeof(Rooms.RoomType));
        public Array AvailabilityOptions => Enum.GetValues(typeof(Rooms.Availability));

        private int _nextRoom;
        public int NextRoom
        {
            get => _nextRoom;
            set { _nextRoom = value; OnPropertyChanged(); }
        }

        private int? _newFloor;
        public int? NewFloor
        {
            get => _newFloor;
            set
            {
                _newFloor = value;
                OnPropertyChanged();
                _ = LoadNextRoom(); 
            }
        }
        public RoomsViewModel()
        {
            LoadRoomsAsync();

        }
        private  async Task LoadNextRoom()
           
        {
            if (NewFloor is null || NewFloor < 1 || NewFloor > 7)
            {
                NextRoom = 0;
                return;
            }
            try
            {
                NextRoom = await _api.GetNextRoom(NewFloor.Value);
                
            }
            catch(Exception e)
            {
                NextRoom = 0;
                MessageBox.Show(e.Message);
            }
            

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
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
