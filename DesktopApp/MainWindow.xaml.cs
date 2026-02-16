using DesktopApp.ViewModels;
using DesktopApp.Views;
using System.Windows;

namespace DesktopApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Mostramos la ventana de login antes de iniciar MainWindow
            var loginWindow = new LoginView();
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult != true)
            {
                // Si el login falla o se cierra, cerramos la aplicación
                Application.Current.Shutdown();
                return;
            }

            // Si login fue exitoso, asignamos el DataContext
            this.DataContext = new MainViewModel();
        }
    }
}
