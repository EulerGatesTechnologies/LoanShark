using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;
using LoanShark.Client;
using LoanShark.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddTransient<AuthMessageHandler>();

// The api endpoint injected by Aspire is usually relative or we can get it from configuration. 
// For now, let's use builder.Configuration["ApiBaseUrl"] if set, otherwise fallback to base address
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;

// IMPORTANT: WebAssembly running on a sub-path or root needs to target the proxy which is at the same origin.
// In this case, our Web project routes /api to the backend. We don't need a full URL if we're on the same origin, 
// just the base address since the proxy handles the `/api` prefix internally, BUT our services append `/api/users/login` etc.
// So the base address should just be the HostEnvironment.BaseAddress.

builder.Services.AddHttpClient<AuthService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<LoanService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<WalletService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
