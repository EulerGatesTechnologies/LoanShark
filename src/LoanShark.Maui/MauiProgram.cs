using Microsoft.Extensions.Logging;
using LoanShark.Web.Services;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace LoanShark.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		// Auth setup
		builder.Services.AddAuthorizationCore();
		builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
		
		// API setup
		builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:17222") });
		
		// Register Services from Web project
		builder.Services.AddScoped<AuthService>();
		builder.Services.AddScoped<LoanService>();
		builder.Services.AddScoped<WalletService>();

		builder.Services.AddFluentUIComponents();

		return builder.Build();
	}
}
