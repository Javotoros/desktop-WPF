using DesktopApp.ViewModels;
using DesktopApp.Views;
using DesktopApp.Views.Reservation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReservasListar_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ListReservationView();

        }
        public void Navigate(UserControl view)
        {
            MainContent.Content = view;
        }
    }
}