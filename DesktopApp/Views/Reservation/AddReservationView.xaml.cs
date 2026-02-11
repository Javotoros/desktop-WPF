using DesktopApp.Models;
using System.Windows;
using DesktopApp.ViewModels;

namespace DesktopApp.Views.Reservation
{
    public partial class AddReservationView : Window
    {
        public AddReservationView()
        {
            InitializeComponent();
            DataContext = new ReservationViewModel(); 
        }

    }
}
