namespace LoanShark.Api.Entities;

public class LedgerTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? FromWalletId { get; set; }
    public Guid? ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Wallet? FromWallet { get; set; }
    public Wallet? ToWallet { get; set; }
}
