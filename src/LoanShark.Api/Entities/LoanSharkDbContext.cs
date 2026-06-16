using Microsoft.EntityFrameworkCore;

namespace LoanShark.Api.Entities;

public class LoanSharkDbContext : DbContext
{
    public LoanSharkDbContext(DbContextOptions<LoanSharkDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<LoanRequest> LoanRequests { get; set; }
    public DbSet<LoanInvestment> LoanInvestments { get; set; }
    public DbSet<LedgerTransaction> LedgerTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure precise money types
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,4)");
        }

        // Unique Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Prevent Cascade Delete Cycles
        modelBuilder.Entity<LoanInvestment>()
            .HasOne(li => li.Lender)
            .WithMany(u => u.Investments)
            .HasForeignKey(li => li.LenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoanRequest>()
            .HasOne(lr => lr.Borrower)
            .WithMany(u => u.LoanRequests)
            .HasForeignKey(lr => lr.BorrowerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<LedgerTransaction>()
            .HasOne(lt => lt.FromWallet)
            .WithMany()
            .HasForeignKey(lt => lt.FromWalletId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<LedgerTransaction>()
            .HasOne(lt => lt.ToWallet)
            .WithMany()
            .HasForeignKey(lt => lt.ToWalletId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
