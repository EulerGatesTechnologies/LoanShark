using System.Net.Http.Json;

namespace LoanShark.Client.Services;

public class ChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SendMessageAsync(string message, List<ChatMessageDto> history)
    {
        var request = new ChatRequest(message, history);
        var response = await _httpClient.PostAsJsonAsync("api/chat", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ChatApiResponse>();
            return result?.Message ?? "No response.";
        }

        return "I'm sorry, I'm having trouble connecting right now.";
    }
}

public record ChatRequest(string Message, List<ChatMessageDto> History);
public record ChatMessageDto(string Role, string Content);
public record ChatApiResponse(string Message);