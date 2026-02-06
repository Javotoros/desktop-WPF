using DesktopApp.Models;
using DesktopApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Views.Reservation
{
    public partial class AddReservationView : UserControl
    {
        private readonly ApiClient _apiClient;

        public AddReservationView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            CargarHabitaciones();
        }

        private async void CargarHabitaciones()
        {
            try
            {
                var rooms = await _apiClient.GetRooms();
                var habitacionesDisponibles = rooms
                    .Where(r => !string.IsNullOrEmpty(r.availability) && r.availability.ToLower() == "available")
                    .ToList();

                lbHabitaciones.ItemsSource = habitacionesDisponibles;
                lbHabitaciones.DisplayMemberPath = "numRoom"; // lo que se muestra
                lbHabitaciones.SelectedValuePath = "Id";      // el valor que usarás
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar habitaciones: " + ex.Message);
            }
        }



        private async void BtnBuscarCliente_Click(object sender, RoutedEventArgs e)
        {
            lblClienteInfo.Text = "Usuario: Juan Perez (Prueba)";

        }

        private async void BtnCrearReserva_Click(object sender, RoutedEventArgs e)
        {
            // Validar selección de habitaciones
            if (lbHabitaciones.SelectedItems == null || lbHabitaciones.SelectedItems.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona al menos una habitación.");
                return;
            }

            // Validar fechas
            if (!dpCheckIn.SelectedDate.HasValue || !dpCheckOut.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, selecciona las fechas de entrada y salida.");
                return;
            }

            if (dpCheckOut.SelectedDate <= dpCheckIn.SelectedDate)
            {
                MessageBox.Show("La fecha de salida debe ser posterior a la de entrada.");
                return;
            }

            try
            {
                // Obtener los IDs de las habitaciones seleccionadas
                var habitacionesSeleccionadas = lbHabitaciones.SelectedItems
                    .Cast<Rooms>()
                    .Select(r => r.Id) // <- Esto usa el Id real de la habitación
                    .ToList();

                if (habitacionesSeleccionadas.Count == 0)
                {
                    MessageBox.Show("Error: ninguna habitación válida seleccionada.");
                    return;
                }

                foreach (var id in habitacionesSeleccionadas)
                {
                    Console.WriteLine("ID seleccionada: " + id);
                }

                // Crear objeto reserva
                var nuevaReserva = new Reservations
                {
                    User = "63f1b2c8a1b2c3d4e5f67890", // usuario fijo para pruebas
                    RoomIds = habitacionesSeleccionadas,
                    CheckIn = dpCheckIn.SelectedDate.Value,
                    CheckOut = dpCheckOut.SelectedDate.Value,
                    Status = ((ComboBoxItem)cbEstado.SelectedItem).Content.ToString()
                };

                // Enviar al API y obtener posible error
                var errorJson = await _apiClient.PostReservationAsync(nuevaReserva);

                if (string.IsNullOrEmpty(errorJson))
                {
                    MessageBox.Show("Reserva creada correctamente.");
                    lbHabitaciones.UnselectAll();
                    dpCheckIn.SelectedDate = null;
                    dpCheckOut.SelectedDate = null;
                    cbEstado.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Error al crear la reserva:\n" + errorJson);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }

    }
}
   

