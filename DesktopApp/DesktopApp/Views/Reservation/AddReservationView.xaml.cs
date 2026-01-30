using DesktopApp.Models;
using DesktopApp.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Views.Reservation
{
    public partial class AddReservationView : UserControl
    {
        private readonly ApiClient _apiClient;
        private User clienteSeleccionado;

        public AddReservationView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            CargarHabitaciones();
         
        }

        private async void CargarHabitaciones()
        {
            /*
            try
            {
                var habitaciones = await _apiClient.GetHabitacionesAsync();
                cbHabitaciones.ItemsSource = habitaciones;
                cbHabitaciones.DisplayMemberPath = "Number";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar habitaciones: " + ex.Message);
            }
            */
        }

        private async void BtnBuscarCliente_Click(object sender, RoutedEventArgs e)
        {
            /*
            var nombre = txtCliente.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                lblClienteInfo.Text = "Escribe un nombre para buscar.";
                clienteSeleccionado = null;
                return;
            }

            try
            {
                // Llamada a la API que busque por nombre (puede devolver 1 usuario o varios)
                var usuario = await _apiClient.BuscarUsuarioPorNombreAsync(nombre);
                if (usuario != null)
                {
                    clienteSeleccionado = usuario;
                    lblClienteInfo.Text = $"Cliente encontrado: {usuario.Name}";
                }
                else
                {
                    clienteSeleccionado = null;
                    lblClienteInfo.Text = "Cliente no encontrado.";
                }
            }
            catch (Exception ex)
            {
                lblClienteInfo.Text = "Error al buscar: " + ex.Message;
            }
            */
        }

        private async void BtnCrearReserva_Click(object sender, RoutedEventArgs e)
        {
          /*
            if (clienteSeleccionado == null)
            {
                MessageBox.Show("Debes seleccionar un cliente válido.");
                return;
            }

            if (cbHabitaciones.SelectedItem == null || dpCheckIn.SelectedDate == null || dpCheckOut.SelectedDate == null)
            {
                MessageBox.Show("Completa todos los campos.");
                return;
            }

            var reserva = new Reservation
            {
                UserId = clienteSeleccionado.Id,
                RoomId = ((Room)cbHabitaciones.SelectedItem).Id,
                CheckIn = dpCheckIn.SelectedDate.Value,
                CheckOut = dpCheckOut.SelectedDate.Value,
                Status = ((ComboBoxItem)cbEstado.SelectedItem).Content.ToString()
            };

            try
            {
                await _apiClient.CrearReservaAsync(reserva);
                MessageBox.Show("Reserva creada correctamente.");

                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                    mainWindow.MainContent.Content = new ListReservationView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear reserva: " + ex.Message);
            }
          */
        }
    }
}
