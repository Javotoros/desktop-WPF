using DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DesktopApp.Views;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ListRoomsView.xaml
    /// </summary>
    public partial class ListRoomsView : UserControl
    {
        public ListRoomsView()
        {
            InitializeComponent();
            DataContext = new RoomsViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormRoomsView form = new FormRoomsView();
            form.Show();
        }
    }
}
