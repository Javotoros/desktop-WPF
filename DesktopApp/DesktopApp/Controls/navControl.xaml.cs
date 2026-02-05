using System.Windows;
using System.Windows.Controls;
using DesktopApp.Views;
using DesktopApp.Views.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp.Controls
{
    public partial class navControl : UserControl
    {
        public navControl()
        {
            InitializeComponent();
            // Opcional: seleccionar algo por defecto
            MainMenu.SelectedIndex = 0;
        }

        private void MainMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainMenu.SelectedItem is not ListBoxItem item) return;
            if (item.Tag is null) return;

            var main = (MainWindow)Application.Current.MainWindow;

            switch (item.Tag.ToString())
            {
                case "dashboard":
                    // main.Navigate(new DashboardView());
                    break;

                case "users":
                    // main.Navigate(new UsersView());
                    break;

                case "reservas":
                    main.Navigate(new ListReservationView());
                    break;

                case "rooms":
                    main.Navigate(new ListRoomsView()); 
                    break;
            }
            if (MainMenu.SelectedItem != null )
            {
                SubMenu.SelectedIndex = -1;
            }
        }

        private void MainMenu2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubMenu.SelectedItem != null)
            {
                MainMenu.SelectedIndex = -1;
            }
        }
    }
}
