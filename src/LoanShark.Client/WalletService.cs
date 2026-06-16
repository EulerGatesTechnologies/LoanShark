using System.Net.Http.Json;

namespace LoanShark.Client.Services;

public class WalletService
{
    private readonly HttpClient _httpClient;

    public WalletService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WalletDto?> GetWalletAsync()
    {
        return await _httpClient.GetFromJsonAsync<WalletDto>("/api/wallet");
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TransactionDto>>("/api/wallet/transactions") ?? new List<TransactionDto>();
    }

    public async Task<bool> DepositAsync(decimal amount)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/wallet/deposit", new { Amount = amount });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> WithdrawAsync(decimal amount)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/wallet/withdraw", new { Amount = amount });
        return response.IsSuccessStatusCode;
    }
}

public class WalletDto
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Direction { get; set; } = string.Empty;
}
