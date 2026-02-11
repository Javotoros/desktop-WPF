using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.Commands;
using System.Windows.Input;
using DesktopApp.Views;
using DesktopApp.Views.Reservas;
using System.Windows;

namespace DesktopApp.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set { 
                _currentView = value; 
                OnPropertyChanged(); 
            }
        }
        private string? _selectedMenu;
        public string? SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                _selectedMenu = value;
                OnPropertyChanged();
            }
        }
        public ICommand NavigateCommand { get; }

        public MainViewModel()
        {
            SelectedMenu = "rooms";
            CurrentView = new ListRoomsView();
            NavigateCommand = new RelayCommand(Navigation);

        }

        public void Navigation(object? p)
        {
            var key = p?.ToString()?.ToLower();

            if (SelectedMenu != key)
                SelectedMenu = key;

            switch (key)
            {
                case "dashboard":
                   //CurrentView = new DashboardView();
                    break;
                case "users":
                    //CurrentView = new UsersView();
                    break;
                case "bookings":
                    CurrentView = new ListReservationView();
                    break;
                case "rooms":
                    CurrentView = new ListRoomsView();
                    break;
                case "help":
                    //CurrentView = new HelpView();
                    break;
                case "power":
                    //CurrentView = new PowerView();
                    break;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
