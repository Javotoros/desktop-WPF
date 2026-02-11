using DesktopApp.Models;
using DesktopApp.Services;
using System.Windows;
using System.Windows.Controls;
using DesktopApp.ViewModels;

namespace DesktopApp.Views.Reservation
{
    public partial class AddReservationView : UserControl
    {
        private readonly ApiClient _apiClient;

        public AddReservationView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            this.DataContext = new ReservationViewModel(); 
        }
        private ReservationViewModel viewModel => (ReservationViewModel)DataContext;

        private async void BtnCrearReserva_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SelectedRooms.Clear();
            foreach (Rooms room in lbHabitaciones.SelectedItems)
                viewModel.SelectedRooms.Add(room);

            await viewModel.CrearReservaAsync();
        }

    }
}
   

