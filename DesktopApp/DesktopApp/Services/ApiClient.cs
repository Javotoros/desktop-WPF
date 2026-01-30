using DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            var response = await _httpClient.GetAsync("/reservations");

            if (!response.IsSuccessStatusCode)
                return new List<Reservations>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Reservations>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public async Task<bool> CancelReservationAsync(string id)
        {
            var url = $"/reservations/delete/{id}";

            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error al cancelar reserva: {error}");
            }
        }


    }
}
