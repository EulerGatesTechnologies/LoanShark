namespace LoanShark.Api.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Wallet? Wallet { get; set; }
    public ICollection<LoanRequest> LoanRequests { get; set; } = new List<LoanRequest>();
    public ICollection<LoanInvestment> Investments { get; set; } = new List<LoanInvestment>();
}
