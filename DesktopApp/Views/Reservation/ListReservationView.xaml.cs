using DesktopApp.Models;
using DesktopApp.Services;
using DesktopApp.ViewModels;
using System.Collections.ObjectModel;
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
            DataContext = new ReservationViewModel();

        }

       
    }
}
