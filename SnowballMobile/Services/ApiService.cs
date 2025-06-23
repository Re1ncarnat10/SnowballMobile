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
            public string Token => _token;
            public ApiService()
            {
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://snowball-bpcnb0bpfcg6fjc8.northeurope-01.azurewebsites.net/api/"),
                    Timeout = TimeSpan.FromSeconds(1500) 

                };
            }

            public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterDto registerDto)
            {
                var content = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("LoginAndRegister/register", content);
                if (response.IsSuccessStatusCode)
                    return (true, null);

                var errorJson = await response.Content.ReadAsStringAsync();

                try
                {
                    var errors = JsonSerializer.Deserialize<List<ApiError>>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (errors != null && errors.Count > 0)
                        return (false, errors[0].Description);
                }
                catch (JsonException)
                {
                    // Ignore JSON parsing errors, return raw error message
                }

                return (false, errorJson);
            }

            private class ApiError
            {
                public string Code { get; set; }
                public string Description { get; set; }
            }

            public async Task<bool> LoginAsync(LoginDto loginDto)
            {
                var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.PostAsync("LoginAndRegister/login", content);
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}, Response: {json}");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {response.ReasonPhrase}");
                    return false;
                }

                var result = JsonSerializer.Deserialize<TokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (string.IsNullOrEmpty(result?.Token))
                {
                    System.Diagnostics.Debug.WriteLine("Brak tokena w odpowiedzi logowania.");
                    return false;
                }

                _token = result.Token;
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

            public async Task<bool> CreateSnowballAsync(SnowballDto snowballDto, Stream? imageFileStream = null, string? imageFileName = null)
            {
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(snowballDto.SnowballId.ToString()), "SnowballId");
                form.Add(new StringContent(snowballDto.Name), "Name");
                form.Add(new StringContent(snowballDto.Description), "Description");
                form.Add(new StringContent(snowballDto.Image), "Image");
                form.Add(new StringContent(snowballDto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Price");

                if (imageFileStream != null && !string.IsNullOrEmpty(imageFileName))
                {
                    var fileContent = new StreamContent(imageFileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    form.Add(fileContent, "imageFile", imageFileName);
                }

                var response = await _httpClient.PostAsync("admin/snowball", form);
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}, Response: {json}");
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> UpdateSnowballAsync(int id, SnowballDto snowballDto, Stream? imageFileStream = null, string? imageFileName = null)
            {
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(snowballDto.SnowballId.ToString()), "SnowballId");
                form.Add(new StringContent(snowballDto.Name), "Name");
                form.Add(new StringContent(snowballDto.Description), "Description");
                form.Add(new StringContent(snowballDto.Image), "Image");
                form.Add(new StringContent(snowballDto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Price");

                if (imageFileStream != null && !string.IsNullOrEmpty(imageFileName))
                {
                    var fileContent = new StreamContent(imageFileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    form.Add(fileContent, "imageFile", imageFileName);
                }

                var response = await _httpClient.PutAsync($"admin/snowball/{id}", form);
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}, Response: {json}");
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
                var response = await _httpClient.PostAsync($"usercart/{userId}/remove/{snowballId}", null);
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[RemoveFromCartAsync] Status: {response.StatusCode}, Content: {content}");
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> ClearCartAsync(string userId)
            {
                var response = await _httpClient.PostAsync($"usercart/{userId}/clear", null);
                return response.IsSuccessStatusCode;
            }

            public async Task<UserCartSummaryDto?> GetCartSummaryAsync(string userId)
            {
                var response = await _httpClient.GetAsync($"usercart/{userId}/summary");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserCartSummaryDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            public async Task<bool> PlaceOrderAsync(string userId)
            {
                var response = await _httpClient.PostAsync($"usercart/{userId}/order", null);
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[PlaceOrderAsync] Status: {response.StatusCode}, Content: {content}");
                return response.IsSuccessStatusCode;
            }
            public async Task<List<OrderDto>> GetOrdersByUserAsync(string userId)
            {
                var response = await _httpClient.GetAsync($"order/user/{userId}");
                if (!response.IsSuccessStatusCode)
                    return new List<OrderDto>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDto>();
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
                    var response = await _httpClient.GetAsync("snowball");
                    System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                    return response.IsSuccessStatusCode;
                }
                catch (HttpRequestException httpEx)
                {
                    System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {httpEx.Message}");
                    if (httpEx.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {httpEx.InnerException.Message}");
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return false;
                }
            }
            private class TokenResponse
            {
                public string Token { get; set; } = string.Empty;
            }
        }
}