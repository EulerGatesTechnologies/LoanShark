using System.Net.Http.Json;

namespace LoanShark.Web.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users/login", new { Email = email, Password = password });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result?.Token;
        }
        return null;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users/register", new { Email = email, Password = password });
        return response.IsSuccessStatusCode;
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}
