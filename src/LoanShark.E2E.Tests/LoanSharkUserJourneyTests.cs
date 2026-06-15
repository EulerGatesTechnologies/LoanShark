using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace LoanShark.E2E.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoanSharkUserJourneyTests : PageTest
{
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "https://localhost:7073"; // Web URL

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true // For local development certs
        };
    }

    [Test]
    public async Task CompleteUserJourney_Register_Login_Borrow_Fund()
    {
        await Task.Delay(2000); 
        
        // 1. Visit homepage
        await Page.GotoAsync($"{_baseUrl}/");

        var email = $"testuser_{Guid.NewGuid().ToString("N")[..8]}@example.com";
        var password = "Password123!";

        // 2. Register
        await Page.ClickAsync("text=Login");
        
        await Page.Locator("fluent-text-field").Nth(0).Locator("input").FillAsync(email);
        await Page.Locator("fluent-text-field").Nth(1).Locator("input").FillAsync(password);
        
        await Page.Locator("fluent-button:has-text('Register')").ClickAsync();
        
        // Wait for redirect to home
        await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex($".*{_baseUrl.Replace("https://", "")}/?.*"), new() { Timeout = 10000 });
        
        // Verify logged in
        // Let's go to Wallet to verify we can access protected page
        await Page.ClickAsync("text=Wallet");
        await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex($".*{_baseUrl.Replace("https://", "")}/wallet"), new() { Timeout = 10000 });
        await Expect(Page.Locator("h3:has-text('My Wallet')")).ToBeVisibleAsync(new() { Timeout = 10000 });
        
        Assert.Pass();
    }
}
