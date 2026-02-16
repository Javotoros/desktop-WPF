using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopApp.ViewModels
{
    public class ReservationCreateViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;

        public ReservationCreateViewModel()
        {
            _apiClient = new ApiClient();

            CrearReservaCommand = new RelayCommand(_ => { _ = CrearReservaAsync(); });

            _ = CargarHabitacionesAsync();
            _ = CargarUsuariosAsync();
        }

        public ObservableCollection<Rooms> Habitaciones { get; } = new();
        public ObservableCollection<Rooms> SelectedRooms { get; } = new();

        public ObservableCollection<User> Usuarios { get; } = new();
        public ObservableCollection<User> UsuariosFiltrados { get; } = new();

        private User _usuarioSeleccionado;
        public User UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set { _usuarioSeleccionado = value; OnPropertyChanged(); }
        }

        private DateTime? _checkIn;
        public DateTime? CheckIn
        {
            get => _checkIn;
            set { _checkIn = value; OnPropertyChanged(); }
        }

        private DateTime? _checkOut;
        public DateTime? CheckOut
        {
            get => _checkOut;
            set { _checkOut = value; OnPropertyChanged(); }
        }

        private string _selectedStatus = "confirmada";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); }
        }

        private string _dniBusqueda;
        public string DNIBusqueda
        {
            get => _dniBusqueda;
            set
            {
                _dniBusqueda = value;
                OnPropertyChanged();
                FiltrarUsuarios();
            }
        }

        public ICommand CrearReservaCommand { get; }

        private async Task CargarHabitacionesAsync()
        {
            try
            {
                var rooms = await _apiClient.GetRooms();

                var disponibles = rooms
                    .Where(r => r.availability != null &&
                                r.availability.ToString().Equals("Available"))
                    .ToList();

                Habitaciones.Clear();
                foreach (var h in disponibles)
                    Habitaciones.Add(h);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar habitaciones: " + ex.Message);
            }
        }

        private async Task CargarUsuariosAsync()
        {
            try
            {
                var lista = await _apiClient.GetUsersByRolAsync("Usuario");

                Usuarios.Clear();
                foreach (var u in lista)
                    Usuarios.Add(u);

                FiltrarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
        }

        private void FiltrarUsuarios()
        {
            var filtro = string.IsNullOrWhiteSpace(DNIBusqueda)
                ? Usuarios
                : new ObservableCollection<User>(
                    Usuarios.Where(u =>
                        !string.IsNullOrEmpty(u.DNI) &&
                        u.DNI.ToUpper().Contains(DNIBusqueda.ToUpper()))
                );

            UsuariosFiltrados.Clear();
            foreach (var u in filtro)
                UsuariosFiltrados.Add(u);

            UsuarioSeleccionado = UsuariosFiltrados.FirstOrDefault();
        }

        private bool PuedeCrearReserva() =>
            SelectedRooms.Count > 0 &&
            CheckIn.HasValue &&
            CheckOut.HasValue &&
            CheckOut > CheckIn;

        public async Task CrearReservaAsync()
        {
            if (UsuarioSeleccionado == null)
            {
                MessageBox.Show("Selecciona un cliente.");
                return;
            }

            if (!PuedeCrearReserva())
            {
                MessageBox.Show($"{SelectedRooms.Count}");
                MessageBox.Show("Completa los datos correctamente.");
                return;
            }

            try
            {
                var nuevaReserva = new Reservations
                {
                    User = UsuarioSeleccionado.Id,
                    RoomIds = SelectedRooms.Select(r => r.Id).ToList(),
                    CheckIn = CheckIn.Value.Date.AddHours(12),
                    CheckOut = CheckOut.Value.Date.AddHours(12),
                    Status = SelectedStatus
                };

                var errorJson = await _apiClient.PostReservationAsync(nuevaReserva);

                if (string.IsNullOrEmpty(errorJson))
                {
                    MessageBox.Show("Reserva creada correctamente.");

                    SelectedRooms.Clear();
                    CheckIn = null;
                    CheckOut = null;
                    SelectedStatus = "confirmada";
                }
                else
                {
                    MessageBox.Show("Error:\n" + errorJson);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
