using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace LoanShark.Web.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public AuthMessageHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch
        {
            // Ignore for prerendering
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
