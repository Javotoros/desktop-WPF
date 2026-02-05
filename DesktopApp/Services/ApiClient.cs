using DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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


        public async Task<List<Reservation>> GetReservasAsync()
        {
            var response = await _httpClient.GetAsync("reservations");

            if (!response.IsSuccessStatusCode)
                return new List<Reservation>();

            var json = await response.Content.ReadAsStringAsync();

            var bookings =  JsonSerializer.Deserialize<List<Reservation>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return bookings;
        }
        public async Task<List<Rooms>> GetRooms()
        {
            var response = await _httpClient.GetAsync("rooms");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var rooms = JsonSerializer.Deserialize<List<Rooms>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
            return rooms ;
        }
        public async Task<int> GetNextRoom(int numFloor)
        {
            var response = await _httpClient.GetAsync($"/rooms/nextRoom/{numFloor}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(json);
            }
            int newRoom = JsonSerializer.Deserialize<int> (json);
            return newRoom;
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
    }
}
