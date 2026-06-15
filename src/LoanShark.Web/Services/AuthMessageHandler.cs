using System.Net.Http.Headers;

namespace LoanShark.Web.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly JwtAuthenticationStateProvider _authStateProvider;

    public AuthMessageHandler(JwtAuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _authStateProvider.CurrentToken;
        
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
