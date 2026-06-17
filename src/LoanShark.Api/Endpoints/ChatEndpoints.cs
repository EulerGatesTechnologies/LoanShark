using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LoanShark.Api.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/chat").WithTags("Chat");

        group.MapPost("/", async ([FromBody] ChatRequest request, IHttpClientFactory httpClientFactory) =>
        {
            var messages = new List<object>
            {
                new { role = "system", content = @"You are the LoanShark AI Interaction Guide. Your job is to help users navigate our peer-to-peer lending app. 
You provide dumb-proof, intuitive instructions.
Keep your answers brief, friendly, and focused on helping users understand borrowing, lending, wallet funding, and how to use the web application." }
            };

            foreach (var msg in request.History)
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }

            messages.Add(new { role = "user", content = request.Message });

            var payload = new
            {
                model = "gpt-4o",
                messages = messages,
                max_tokens = 500
            };

            var client = httpClientFactory.CreateClient("GitHubModels");
            
            if (client.DefaultRequestHeaders.Authorization == null)
            {
                return Results.Ok(new ChatApiResponse("Please configure GitHubModels:Token to enable the AI Guide."));
            }

            var response = await client.PostAsJsonAsync("/chat/completions", payload);
            if (!response.IsSuccessStatusCode)
            {
                return Results.Ok(new ChatApiResponse($"AI service returned error: {response.StatusCode}"));
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var text = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";

            return Results.Ok(new ChatApiResponse(text));
        });
    }
}

public record ChatRequest(string Message, List<ChatMessageDto> History);
public record ChatMessageDto(string Role, string Content);
public record ChatApiResponse(string Message);
