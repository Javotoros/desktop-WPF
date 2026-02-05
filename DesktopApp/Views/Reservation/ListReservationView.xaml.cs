using DesktopApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using DesktopApp.Models;

namespace DesktopApp.Views.Reservas
{
    public partial class ListReservationView : UserControl
    {
        private readonly ApiClient _apiClient;
        public ObservableCollection<Reservation> Reservas { get; set; }

        public ListReservationView()
        {
            InitializeComponent();

            _apiClient = new ApiClient();
            Reservas = new ObservableCollection<Reservation>();
            dgReservas.ItemsSource = Reservas;

            CargarReservas();
        }

        private async void CargarReservas()
        {
            try
            {
                var lista = await _apiClient.GetReservasAsync();

                Reservas.Clear();
                foreach (var r in lista)
                    Reservas.Add(r);
            }
            catch (Exception ex)
            {
                // Aquí puedes mostrar un mensaje de error
                System.Windows.MessageBox.Show("Error al cargar reservas: " + ex.Message);
            }
        }
    }
}


