using DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using static DesktopApp.Models.Rooms;

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
            var response = await _httpClient.GetAsync($"rooms/nextRoom/{numFloor}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(json);
            }
            int newRoom = JsonSerializer.Deserialize<int> (json);
            return newRoom;
        }
        public async Task<string> PostRooms(int numFloor, string roomType, string description, int pricePerNight, int maxOccupancy, string availability)
        {
            var values = new Dictionary<string, string>()
                {
                    { "numFloor", numFloor.ToString()},
                    { "roomType",roomType.ToLower()},
                    { "description",description},
                    { "pricePerNight",pricePerNight.ToString()},
                    { "maxOccupancy",maxOccupancy.ToString()},
                    { "availability",availability.ToLower()}
                 };
            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync("rooms/add", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"{(int)response.StatusCode} {response.ReasonPhrase}\n{body}");

            return body;
        }

        public async Task updateIdRoom(string id, string roomType, string description, int pricePerNight, int maxOccupancy, string availability)
        {
            var values = new Dictionary<string, string>()
                {
                    { "roomType",roomType.ToLower()},
                    { "description",description},
                    { "pricePerNight",pricePerNight.ToString()},
                    { "maxOccupancy",maxOccupancy.ToString()},
                    { "availability",availability.ToLower()}
                 };
            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PatchAsync($"rooms/modify/{id}", content);
            var json = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
                throw new Exception($"{(int)response.StatusCode} {response.ReasonPhrase}");
           
        }
        public async Task DeleteIdRoom(string id)
        {
            var response = await _httpClient.DeleteAsync($"rooms/delete/{id}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(json);
            }
        }
        public async Task DeleteIdImgRoom(string id, string imagePath)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"rooms/delete/{id}/images")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new { image = imagePath }),
                    Encoding.UTF8,
                    "application/json"
                )
}
            ;
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        public async Task PostIdImgRoom(string id, List<string> filePaths)
        {
            using var form = new MultipartFormDataContent();

            foreach (var p in filePaths)
            {
                var bytes = await File.ReadAllBytesAsync(p);
                var content = new ByteArrayContent(bytes);

                var ext = Path.GetExtension(p).ToLowerInvariant();

                string type;

                switch (ext)
                {
                    case ".jpg":
                    case ".jpeg":
                        type = "image/jpeg";
                        break;

                    case ".png":
                        type = "image/png";
                        break;

                    case ".webp":
                        type = "image/webp";
                        break;

                    default:
                        type = "application/octet-stream";
                        break;
                }

                content.Headers.ContentType =new System.Net.Http.Headers.MediaTypeHeaderValue(type);

                form.Add(content, "images", Path.GetFileName(p)); 
            }

            var response = await _httpClient.PostAsync($"rooms/add/{id}/images",form);
            response.EnsureSuccessStatusCode();
            
        }

        public async Task<List<Reviews>> GetReviewIdRoom(string id)
        {
            var response = await _httpClient.GetAsync($"reviews/room/{id}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(json);
            }
            var reviews = JsonSerializer.Deserialize<List<Reviews>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return reviews;
        }
    }
}
