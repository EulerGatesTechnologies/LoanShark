namespace LoanShark.Api.Entities;

public class LoanInvestment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LoanRequestId { get; set; }
    public Guid LenderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public LoanRequest? LoanRequest { get; set; }
    public User? Lender { get; set; }
}
