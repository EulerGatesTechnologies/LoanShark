namespace LoanShark.Api.Entities;

public class LoanRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BorrowerId { get; set; }
    public decimal AmountRequested { get; set; }
    public decimal AmountFunded { get; set; }
    public decimal InterestRate { get; set; }
    public int TermDays { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? Borrower { get; set; }
    public ICollection<LoanInvestment> Investments { get; set; } = new List<LoanInvestment>();
}
