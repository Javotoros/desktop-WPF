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
    public class ReservationListViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;

        public ReservationListViewModel()
        {
            _apiClient = new ApiClient();

            LimpiarCommand = new RelayCommand(_ => LimpiarFiltro());
            NuevaReservaCommand = new RelayCommand(_ => NuevaReserva());
            EliminarReservaCommand = new RelayCommand(_ => EliminarReserva());
            CancelarReservaCommand = new RelayCommand(async _ => await CancelarReservaAsync());

            _ = CargarReservasAsync();
        }

        public ObservableCollection<Reservations> Reservas { get; } = new();
        private ObservableCollection<Reservations> _todas;

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
                _ocultarCanceladas = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        public ICommand NuevaReservaCommand { get; }
        public ICommand CancelarReservaCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand EliminarReservaCommand { get; }

        public async Task CargarReservasAsync()
        {
            try
            {
                var reservations = await _apiClient.GetReservasAsync();
                var usuarios = await _apiClient.GetUsersByRolAsync("Usuario");

                foreach (var reservation in reservations)
                {
                    var rooms = await Task.WhenAll(
                        reservation.RoomIds.Select(id => _apiClient.GetRoomsId(id))
                    );

                    reservation.Rooms = rooms.Where(r => r != null).ToList();

                    var user = usuarios.FirstOrDefault(u => u.Id == reservation.User);
                    if (user != null)
                    {
                        reservation.UserDNI = user.DNI;
                        reservation.UserNombre = user.NombreCompleto;
                    }
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
            var ventana = new Views.Reservation.AddReservationView();
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();
        }

        private void EliminarReserva()
        {
            var ventana = new Views.Reservation.DeleteCancelledReservationsView();
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

