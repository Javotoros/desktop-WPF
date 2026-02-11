using DesktopApp.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.IO;
using System.Text.Json.Serialization;

namespace DesktopApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;


        // URL base de tu API
        private const string BASE_URL = "http://localhost:3000/";

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BASE_URL);
        }


        public async Task<List<Reservations>> GetReservasAsync()
        {
            var response = await _httpClient.GetAsync("reservations");

            if (!response.IsSuccessStatusCode)
                return new List<Reservations>();

            var json = await response.Content.ReadAsStringAsync();

            var bookings =  JsonSerializer.Deserialize<List<Reservations>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return bookings ?? new List<Reservations>();
        }

        public async Task<bool> CancelReservationAsync(string reservationId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"reservations/delete/{reservationId}");

                // Depuración:
                if (!response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error API: {response.StatusCode}\n{contenido}");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Rooms>> GetRooms()
        {
            var response = await _httpClient.GetAsync("rooms");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var rooms = JsonSerializer.Deserialize<List<Rooms>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return rooms ?? new List<Rooms>();
        }

        public async Task<Rooms> GetRoomsId(string id)
        {
            var response = await _httpClient.GetAsync($"rooms/{id}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var rooms = JsonSerializer.Deserialize<Rooms>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return rooms;
        }

        public async Task<bool> PostRooms(int numFloor, string roomType, string description,
                    string image, int pricePerNight, string reviews, int maxOccupancy, string availability)
        {
            try
            {
                var values = new Dictionary<string, string>()
                {
                    { "numFloor", numFloor.ToString()},
                    { "roomType",roomType},
                    { "description",description},
                    { "image",image},
                    { "pricePerNight",pricePerNight.ToString()},
                    { "reviews",reviews},
                    { "maxOccupancy",maxOccupancy.ToString()},
                    { "availability",availability}
                 };
                var content = new FormUrlEncodedContent(values);
                var response = await _httpClient.PostAsync("add", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }


        }

        public async Task<string> PostReservationAsync(Reservations reserva)
        {
            var json = JsonSerializer.Serialize(reserva);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("reservations/add", content);

            var respuestaJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return null; 
            else
                return respuestaJson;
        }

<<<<<<< HEAD
    }
=======
        public async Task<List<User>> GetUsersByRolAsync(string rol)
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/rol/{rol}");
                if (!response.IsSuccessStatusCode)
                    return new List<User>();

                var json = await response.Content.ReadAsStringAsync();

                var users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener usuarios: " + ex.Message);
                return new List<User>();
            }
        }

    }





>>>>>>> 3437da2 (fix: CRUD funcional)
}
