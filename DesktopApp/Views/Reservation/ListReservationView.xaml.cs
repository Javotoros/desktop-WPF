using DesktopApp.ViewModels;
using System.Windows.Controls;

namespace DesktopApp.Views.Reservation
{
    public partial class ListReservationView : UserControl
    {
        public ListReservationView()
        {
            InitializeComponent();
            DataContext = new ReservationViewModel();
        }
    }
}
