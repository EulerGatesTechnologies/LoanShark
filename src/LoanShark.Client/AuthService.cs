using System.Net.Http.Json;

namespace LoanShark.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool Success, string ErrorMessage)> RegisterAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/users/register", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
                return (true, string.Empty);
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(errorContent) ? $"API returned {response.StatusCode}" : errorContent);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    public async Task<(string? Token, string ErrorMessage)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/users/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return (result?.Token, string.Empty);
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return (null, string.IsNullOrWhiteSpace(errorContent) ? $"API returned {response.StatusCode}" : errorContent);
        }
        catch (Exception ex)
        {
            return (null, $"Network error: {ex.Message}");
        }
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}
