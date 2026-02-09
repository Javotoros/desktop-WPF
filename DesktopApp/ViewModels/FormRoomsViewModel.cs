using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using System.Windows.Input;
using System.Windows;
using System.Text.Json;

namespace DesktopApp.ViewModels
{
    public  class FormRoomsViewModel:INotifyPropertyChanged
    {
        private readonly ApiClient _api = new ApiClient();
        public event PropertyChangedEventHandler? PropertyChanged;

        public Array RoomTypes => Enum.GetValues(typeof(Rooms.RoomType));
        public Array AvailabilityOptions => Enum.GetValues(typeof(Rooms.Availability));

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }

        private int? _currentRoom;
        public int? CurrentRoom
        {
            get => _currentRoom;
            set { _currentRoom = value; OnPropertyChanged(); }
        }

        private Rooms? _selectedRoom;
        public Rooms? SelectedRoom
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(); }
        }

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
                CommandManager.InvalidateRequerySuggested();
            }
        }


        private Rooms.RoomType? _roomType;
        public Rooms.RoomType? RoomType
        {
            get => _roomType;
            set
            {
                _roomType = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private int? _pricePerNight;
        public int? PricePerNight
        {
            get => _pricePerNight;
            set
            {
                _pricePerNight = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string? _image;
        public string? Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }
        private string? _reviews;
        public string? Reviews
        {
            get => _reviews;
            set
            {
                _reviews = value;
                OnPropertyChanged();
            }
        }

        private int? _maxOccupancy;
        public int? MaxOccupancy
        {
            get => _maxOccupancy;
            set
            {
                _maxOccupancy = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Rooms.Availability? _availability;
        public Rooms.Availability? Availability
        {
            get => _availability;
            set
            {
                _availability = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand CancelCommand { get; }


        public FormRoomsViewModel()
        {
            IsEditing = true;
            SaveCommand = new RelayCommand(async _ => await SendDataRooms(), _ => CanSave());
            CancelCommand = new RelayCommand(w => CloseWindow(w as Window));
            LimpiarCommand = new RelayCommand(_ => Clean());

        }
        public FormRoomsViewModel(Rooms room):this()
        {
            SelectedRoom = room;
            IsEditing = false;
            CurrentRoom = room.numRoom;
            NewFloor = room.numFloor;
            RoomType = room.roomType;
            Description = room.description;
            PricePerNight = (int)room.pricePerNight;
            MaxOccupancy = room.maxOccupancy;
            Availability = room.availability;
            Image = room.image?.FirstOrDefault() ?? "";
            Reviews = room.reviews?.FirstOrDefault() ?? "";

            SaveCommand = new RelayCommand(async w => await UpdateDataRooms(w as Window), _ => CanSave());
            LimpiarCommand = new RelayCommand(_ => CleanUpdate());
        }
       
        private bool CanSave()
        {
            if (NewFloor.HasValue
                && NewFloor.Value >= 1 && NewFloor.Value <= 7
                && PricePerNight.HasValue && PricePerNight.Value >= 1
                && MaxOccupancy.HasValue && MaxOccupancy.Value >= 1 && MaxOccupancy.Value <= 4
                && RoomType.HasValue
                && Availability.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Clean()
        {
            NewFloor = null;
            RoomType = null;
            Description = null;
            Image = null;
            PricePerNight = null;
            Reviews = null;
            MaxOccupancy = null;
            Availability = null;

            CommandManager.InvalidateRequerySuggested();
        }
        private void CleanUpdate()
        {
            RoomType = null;
            Description = null;
            Image = null;
            PricePerNight = null;
            Reviews = null;
            MaxOccupancy = null;
            Availability = null;

            CommandManager.InvalidateRequerySuggested();
        }

        private async Task LoadNextRoom()

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
            catch (Exception e)
            {
                NextRoom = 0;
                MessageBox.Show(e.Message);
            }


        }


        private async Task SendDataRooms()
        {
            try
            {
                string body = await _api.PostRooms(NewFloor!.Value,
                    RoomType.Value.ToString(),
                    Description ?? "",
                    Image ?? "",
                    PricePerNight!.Value,
                    Reviews ?? "",
                    MaxOccupancy!.Value,
                    Availability.Value.ToString()
                );
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                MessageBox.Show(
                    $"{root.GetProperty("message").GetString()}\n\n" +
                    $"Número habitación: {root.GetProperty("numRoom").GetInt32()}\n" +
                    $"Planta: {root.GetProperty("numFloor").GetInt32()}",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                Clean();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private async Task UpdateDataRooms(Window? w)
        {
            try
            {
                    await _api.updateIdRoom(SelectedRoom!.Id,
                    RoomType.Value.ToString(),
                    Description ?? "",
                    Image ?? "",
                    PricePerNight!.Value,
                    Reviews ?? "",
                    MaxOccupancy!.Value,
                    Availability.Value.ToString()
                );
                MessageBox.Show(
                    "Habitación actualizada\n\n" +
                    $"Número habitación: {SelectedRoom.numRoom}\n" +
                    $"Planta: {SelectedRoom.numFloor}",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                CloseWindow(w);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        

        private void CloseWindow(Window? w)
        {
            w?.Close();
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
