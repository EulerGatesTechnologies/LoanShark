using System.ComponentModel.DataAnnotations;

namespace LoanShark.Api.Entities;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public User? User { get; set; }
}
