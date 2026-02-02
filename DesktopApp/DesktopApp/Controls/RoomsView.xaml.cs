using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DesktopApp.ViewModels;

namespace DesktopApp.Controls
{
    /// <summary>
    /// Lógica de interacción para RoomsView.xaml
    /// </summary>
    public partial class RoomsView : UserControl
    {
        public RoomsView()
        {
            InitializeComponent();
            DataContext = new RoomsViewModel();
        }
    }
}
