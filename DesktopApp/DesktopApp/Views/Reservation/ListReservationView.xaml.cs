using DesktopApp.Models;
using DesktopApp.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Views.Reservation
{
    public partial class ListReservationView : UserControl
    {
        private readonly ApiClient _apiClient;
        public ObservableCollection<Reservations> Reservas { get; set; }

        public ListReservationView()
        {
            InitializeComponent();

            _apiClient = new ApiClient();
            Reservas = new ObservableCollection<Reservations>();
            dgReservation.ItemsSource = Reservas;

            CargarReservas();
            _ = LoadReservationsAsync();

        }

        private async void CargarReservas()
        {
            try
            {
                var lista = await _apiClient.GetReservasAsync();
                Reservas.Clear();

                foreach (var r in lista)
                {
                    Reservas.Add(r);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reservas: " + ex.Message);
            }
        }


        private void BtnNuevaReserva_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var nuevaReservaView = new AddReservationView(); 
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = nuevaReservaView;
            }
        }

        private async void BtnCancelarReserva_Click(object sender, RoutedEventArgs e)
        {
            if (dgReservation.SelectedItem != null)
            {
                var reservaSeleccionada = (Reservations)dgReservation.SelectedItem;

                try
                {
                    bool exito = await _apiClient.CancelReservationAsync(reservaSeleccionada.Id);
                    if (exito)
                    {
                        reservaSeleccionada.Status = "cancelada";

                        dgReservation.Items.Refresh();

                        dgReservation.SelectedItem = null;
                        MessageBox.Show($"Reserva con Id: {reservaSeleccionada.Id} cancelada correctamente.");
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
            else
            {
                MessageBox.Show("Debes seleccionar una reserva para cancelar.");
            }
        }


        public async Task LoadReservationsAsync()
        {
            var reservations = await _apiClient.GetReservasAsync();

            foreach (var reservation in reservations)
            {
                var rooms = await Task.WhenAll(reservation.RoomIds.Select(id => _apiClient.GetRoomsId(id)));
                reservation.Rooms = rooms.Where(r => r != null).ToList();
            }

            Reservas.Clear();
            foreach (var r in reservations)
                Reservas.Add(r);
        }
    }
}


