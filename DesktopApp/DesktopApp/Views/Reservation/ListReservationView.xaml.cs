using DesktopApp.Models;
using DesktopApp.Services;
using DesktopApp.Views.Reservation;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DesktopApp.Models;

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
        }

        private async void CargarReservas()
        {
            var lista = await _apiClient.GetReservasAsync();
            try
            {

                Reservas.Clear();
                foreach (var r in lista)
                    Reservas.Add(r);
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

    }
}


