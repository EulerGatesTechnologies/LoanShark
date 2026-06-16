using System.Net.Http.Json;

namespace LoanShark.Client.Services;

public class LoanService
{
    private readonly HttpClient _httpClient;

    public LoanService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<LoanDto>> GetLoansAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<LoanDto>>("/api/loans") ?? new List<LoanDto>();
    }

    public async Task<LoanDto?> GetLoanAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<LoanDto>($"/api/loans/{id}");
    }

    public async Task<bool> CreateLoanAsync(decimal amount, decimal interestRate, int termDays)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/loans", new { Amount = amount, InterestRate = interestRate, TermDays = termDays });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> FundLoanAsync(Guid id, decimal amount)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/loans/{id}/fund", new { Amount = amount });
        return response.IsSuccessStatusCode;
    }
}

public class LoanDto
{
    public Guid Id { get; set; }
    public Guid BorrowerId { get; set; }
    public decimal AmountRequested { get; set; }
    public decimal AmountFunded { get; set; }
    public decimal InterestRate { get; set; }
    public int TermDays { get; set; }
    public string Status { get; set; } = string.Empty;
}
