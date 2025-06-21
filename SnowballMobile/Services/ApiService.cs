using SnowballMobile.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SnowballMobile.Services
{
    public class ApiService
        {
            private readonly HttpClient _httpClient;
            private string _token = string.Empty;

            public ApiService()
            {
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri("http://10.0.2.2:5294/api/"),
                    Timeout = TimeSpan.FromSeconds(1500) 

                };
            }

            public async Task<bool> RegisterAsync(RegisterDto registerDto)
            {
                var content = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("LoginAndRegister/register", content);
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> LoginAsync(LoginDto loginDto)
            {
                var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("LoginAndRegister/login", content);

                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}, Response: {json}");

                if (!response.IsSuccessStatusCode) return false;

                var result = JsonSerializer.Deserialize<TokenResponse>(json);
                _token = result?.Token ?? string.Empty;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                return true;
            }

            public async Task<List<SnowballDto>> GetAllSnowballsAsync()
            {
                var response = await _httpClient.GetAsync("snowball");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SnowballDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            public async Task<SnowballDto?> GetSnowballByIdAsync(int id)
            {
                var response = await _httpClient.GetAsync($"snowball/{id}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SnowballDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            public async Task<bool> CreateSnowballAsync(SnowballDto snowballDto)
            {
                var content = new StringContent(JsonSerializer.Serialize(snowballDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("admin/snowball", content);
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> UpdateSnowballAsync(int id, SnowballDto snowballDto)
            {
                var content = new StringContent(JsonSerializer.Serialize(snowballDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"admin/snowball/{id}", content);
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> DeleteSnowballAsync(int id)
            {
                var response = await _httpClient.DeleteAsync($"admin/snowball/{id}");
                return response.IsSuccessStatusCode;
            }
            
            public async Task<bool> AddToCartAsync(string userId, int snowballId)
            {
                var response = await _httpClient.PostAsync($"usercart/{userId}/add/{snowballId}", null);
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> RemoveFromCartAsync(string userId, int snowballId)
            {
                var response = await _httpClient.DeleteAsync($"usercart/{userId}/remove/{snowballId}");
                return response.IsSuccessStatusCode;
            }

            public async Task<UserCartSummaryDto?> GetCartSummaryAsync(string userId)
            {
                var response = await _httpClient.GetAsync($"usercart/{userId}/summary");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserCartSummaryDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            public async Task<bool> ClearCartAsync(string userId)
            {
                var response = await _httpClient.DeleteAsync($"usercart/{userId}/clear");
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> PlaceOrderAsync(string userId)
            {
                var response = await _httpClient.PostAsync($"usercart/{userId}/order", null);
                return response.IsSuccessStatusCode;
            }
            
            public async Task<List<OrderDto>> GetAllOrdersAsync()
            {
                var response = await _httpClient.GetAsync("order");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
            {
                var response = await _httpClient.GetAsync($"order/{orderId}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OrderDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            public async Task<bool> InitializeAdminAsync()
            {
                var response = await _httpClient.PostAsync("admin/initialize", null);
                return response.IsSuccessStatusCode;
            }
            public async Task<bool> TestConnectionAsync()
            {
                try
                {
                    // Możesz zmienić "snowball" na inny prosty endpoint, np. "health" jeśli taki istnieje
                    var response = await _httpClient.GetAsync("snowball");
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
            private class TokenResponse
            {
                public string Token { get; set; } = string.Empty;
            }
        }
}