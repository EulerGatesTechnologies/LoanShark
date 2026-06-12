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
    }
}
