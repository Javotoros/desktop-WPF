using DesktopApp.Models;
using DesktopApp.Services;
using DesktopApp.Views.Reservation;
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
        public Reservations ReservaSeleccionada { get; set; }

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
                    AplicarFiltro(); // reaplica el filtro al cambiar el valor
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


        public ReservationViewModel()
        {
            _apiClient = new ApiClient();

            BuscarCommand = new RelayCommand(AplicarFiltro);
            LimpiarCommand = new RelayCommand(LimpiarFiltro);

            NuevaReservaCommand = new RelayCommand(NuevaReserva);
            CancelarReservaCommand = new RelayCommand(CancelarReserva, PuedeCancelar);

            BuscarClienteCommand = new RelayCommand(BuscarCliente);
            CrearReservaCommand = new RelayCommand(async () => await CrearReservaAsync());
            VolverCommand = new RelayCommand(Volver);

            _ = CargarHabitacionesAsync();

            _ = CargarReservasAsync();
        }

        //===============================
        //==========LISTAR===============
        //===============================

        private async Task CargarReservasAsync()
        {
            var reservations = await _apiClient.GetReservasAsync();

            foreach (var reservation in reservations)
            {
                var rooms = await Task.WhenAll(reservation.RoomIds.Select(id => _apiClient.GetRoomsId(id)));
                reservation.Rooms = rooms.Where(r => r != null).ToList();
            }

            _todas = new ObservableCollection<Reservations>(reservations);

            Reservas.Clear();
            foreach (var r in reservations)
                Reservas.Add(r);

            AplicarFiltro();

        }

        private void AplicarFiltro()
        {
            if (_todas == null) return;

            var filtradas = _todas.AsEnumerable();

            // Filtro por texto (habitaciones)
            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                filtradas = filtradas.Where(r =>
                    r.Rooms != null &&
                    r.Rooms.Any(h =>
                        h.numRoom != null &&
                        h.numRoom.ToString().Contains(TextoBusqueda)
                    )
                );
            }

            // Filtro para ocultar canceladas
            if (OcultarCanceladas)
            {
                filtradas = filtradas.Where(r => r.Status.ToLower() != "cancelada");
            }

            Reservas.Clear();
            foreach (var r in filtradas)
                Reservas.Add(r);
        }


        private void LimpiarFiltro()
        {
            TextoBusqueda = "";
            OcultarCanceladas = true;

            Reservas.Clear();
            foreach (var r in _todas)
                Reservas.Add(r);

            AplicarFiltro();
        }

        private void NuevaReserva()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new AddReservationView();
            }
        }

        private bool PuedeCancelar() => ReservaSeleccionada != null;

        private async void CancelarReserva()
        {
            if (ReservaSeleccionada == null)
                return;

            try
            {
                bool exito = await _apiClient.CancelReservationAsync(ReservaSeleccionada.Id);
                if (exito)
                {
                    ReservaSeleccionada.Status = "cancelada";
                    OnPropertyChanged(nameof(Reservas));
                    MessageBox.Show($"Reserva con Id: {ReservaSeleccionada.Id} cancelada correctamente.");
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
                    .Where(r => !string.IsNullOrEmpty(r.availability) && r.availability.ToLower() == "available")
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
            //BUSQUEDA DEL CLIENTE
            ClienteInfo = "Usuario: Juan Perez (Prueba)";
        }

        public async Task CrearReservaAsync()
        {
            if (SelectedRooms.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona al menos una habitación.");
                return;
            }

            if (!CheckIn.HasValue || !CheckOut.HasValue)
            {
                MessageBox.Show("Por favor, selecciona las fechas de entrada y salida.");
                return;
            }

            if (CheckOut <= CheckIn)
            {
                MessageBox.Show("La fecha de salida debe ser posterior a la de entrada.");
                return;
            }

            try
            {
                var roomIds = SelectedRooms.Select(r => r.Id).ToList();

                var nuevaReserva = new Reservations
                {
                    User = "63f1b2c8a1b2c3d4e5f67890", // usuario fijo para pruebas
                    RoomIds = roomIds,
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
                    SelectedStatus = null;
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
            var nuevaListaView = new Views.Reservation.ListReservationView();
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
                mainWindow.MainContent.Content = nuevaListaView;
        }


        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
}
