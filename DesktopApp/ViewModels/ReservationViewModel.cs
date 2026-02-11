using DesktopApp.Commands; // Importa AsyncRelayCommand
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
    public class ReservationViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;

        //========LISTAR================
        private Reservations _reservaSeleccionada;
        public Reservations ReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set
            {
                _reservaSeleccionada = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand NuevaReservaCommand { get; }
        public ICommand CancelarReservaCommand { get; }
        public ICommand BuscarCommand { get; }
        public ICommand LimpiarCommand { get; }

        public ObservableCollection<Reservations> Reservas { get; } = new();
        private ObservableCollection<Reservations> _todas;

        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private bool _ocultarCanceladas = true;
        public bool OcultarCanceladas
        {
            get => _ocultarCanceladas;
            set
            {
                if (_ocultarCanceladas != value)
                {
                    _ocultarCanceladas = value;
                    OnPropertyChanged();
                    AplicarFiltro();
                }
            }
        }

        //========CREAR================
        public ObservableCollection<Rooms> Habitaciones { get; } = new ObservableCollection<Rooms>();
        public ObservableCollection<Rooms> SelectedRooms { get; set; } = new ObservableCollection<Rooms>();

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

        private string _clienteInfo;
        public string ClienteInfo
        {
            get => _clienteInfo;
            set { _clienteInfo = value; OnPropertyChanged(); }
        }

        public ICommand BuscarClienteCommand { get; }
        public ICommand CrearReservaCommand { get; }
        public ICommand VolverCommand { get; }

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

        public ObservableCollection<User> Usuarios { get; } = new ObservableCollection<User>();
        public ObservableCollection<User> UsuariosFiltrados { get; } = new ObservableCollection<User>();

        private User _usuarioSeleccionado;
        public User UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set { _usuarioSeleccionado = value; OnPropertyChanged(); }
        }


        /// ==============================
        /// =========CONSTRUCTOR==========
        /// ==============================

        public ReservationViewModel()
        {
            _apiClient = new ApiClient();

            //Comandos síncronos 
            BuscarCommand = new RelayCommand(_ => AplicarFiltro());
            LimpiarCommand = new RelayCommand(_ => LimpiarFiltro());
            NuevaReservaCommand = new RelayCommand(_ => NuevaReserva());
            BuscarClienteCommand = new RelayCommand(_ => BuscarCliente());
            VolverCommand = new RelayCommand(_ => Volver());

            CancelarReservaCommand = new RelayCommand(
                async _ => await CancelarReservaAsync()
            );


            CrearReservaCommand = new RelayCommand(_ => { _ = CrearReservaAsync(); });

            _ = CargarHabitacionesAsync();
            _ = CargarReservasAsync();
            _ = CargarUsuariosAsync();

        }

        //===============================
        //==========LISTAR===============
        //===============================

        private async Task CargarReservasAsync()
        {
            try
            {
                var reservations = await _apiClient.GetReservasAsync();

                // Cargar habitaciones para cada reserva
                foreach (var reservation in reservations)
                {
                    var rooms = await Task.WhenAll(
                        reservation.RoomIds.Select(id => _apiClient.GetRoomsId(id))
                    );
                    reservation.Rooms = rooms.Where(r => r != null).ToList();
                }

                _todas = new ObservableCollection<Reservations>(reservations);
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reservas: " + ex.Message);
            }
        }

        private void AplicarFiltro()
        {
            if (_todas == null) return;

            var filtradas = _todas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                filtradas = filtradas.Where(r =>
                    r.Rooms?.Any(h =>
                        h.numRoom.ToString().Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
                    ) == true
                );
            }

            if (OcultarCanceladas)
            {
                filtradas = filtradas.Where(r =>
                    !string.Equals(r.Status, "cancelada", StringComparison.OrdinalIgnoreCase)
                );
            }

            Reservas.Clear();
            foreach (var r in filtradas)
                Reservas.Add(r);
        }

        private void LimpiarFiltro()
        {
            TextoBusqueda = "";
            OcultarCanceladas = true;
            AplicarFiltro();
        }

        private void NuevaReserva()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow?.DataContext is MainViewModel vm)
            {
                vm.CurrentView = new Views.Reservation.AddReservationView();
            }
        }
        private async Task CancelarReservaAsync()
        {
            if (ReservaSeleccionada == null) return;

            try
            {
                bool exito = await _apiClient.CancelReservationAsync(ReservaSeleccionada.Id);
                if (exito)
                {
                    ReservaSeleccionada.Status = "cancelada";
                    MessageBox.Show($"Reserva ID: {ReservaSeleccionada.Id} cancelada correctamente.");
                    AplicarFiltro(); 
                }
                else
                {
                    MessageBox.Show("No se pudo cancelar la reserva.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cancelar reserva: " + ex.Message);
            }
        }

        //===============================
        //==========CREAR===============
        //===============================

        private async Task CargarHabitacionesAsync()
        {
            try
            {
                var rooms = await _apiClient.GetRooms();
                var disponibles = rooms
                    .Where(r =>
                        r.availability != null &&
                        r.availability.ToString().Equals("Available")
                    )
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

        private void BuscarCliente()
        {
            ClienteInfo = "Usuario: Juan Perez (Prueba)";
        }

        private bool PuedeCrearReserva() =>
            SelectedRooms.Count > 0 &&
            CheckIn.HasValue &&
            CheckOut.HasValue &&
            CheckOut > CheckIn;

        public async Task CrearReservaAsync()
        {
            if (!PuedeCrearReserva())
            {
                MessageBox.Show("Completa todos los campos correctamente.");
                return;
            }

            try
            {
                var roomIds = SelectedRooms.Select(r => r.Id).ToList();

                var nuevaReserva = new Reservations
                {
                    User = "63f1b2c8a1b2c3d4e5f67890",
                    RoomIds = roomIds,
                    CheckIn = CheckIn.Value.Date.AddHours(12),
                    CheckOut = CheckOut.Value.Date.AddHours(12),
                    Status = SelectedStatus ?? "confirmada"
                };

                var errorJson = await _apiClient.PostReservationAsync(nuevaReserva);

                if (string.IsNullOrEmpty(errorJson))
                {
                    MessageBox.Show("Reserva creada correctamente.");

                    // Limpiar formulario
                    SelectedRooms.Clear();
                    CheckIn = null;
                    CheckOut = null;
                    SelectedStatus = "confirmada";

                    // Recargar lista de reservas
                    await CargarReservasAsync();
                }
                else
                {
                    MessageBox.Show("Error al crear la reserva:\n" + errorJson);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }

        private void Volver()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow?.DataContext is MainViewModel vm)
            {
                vm.CurrentView = new Views.Reservation.ListReservationView();
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
            if (Usuarios == null) return;

            // Usar ToUpperInvariant() para que la búsqueda no sea case sensitive
            var filtro = string.IsNullOrWhiteSpace(DNIBusqueda)
                ? Usuarios
                : new ObservableCollection<User>(
                    Usuarios.Where(u => !string.IsNullOrEmpty(u.DNI) &&
                                        u.DNI.ToUpperInvariant().Contains(DNIBusqueda.ToUpperInvariant()))
                );

            //Limpiar y volver a agregar
            UsuariosFiltrados.Clear();
            foreach (var u in filtro)
                UsuariosFiltrados.Add(u);
        }



        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}