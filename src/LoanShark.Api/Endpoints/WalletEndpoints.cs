using System.Security.Claims;
using LoanShark.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanShark.Api.Endpoints;

public static class WalletEndpoints
{
    public static void MapWalletEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wallet").WithTags("Wallet").RequireAuthorization();

        group.MapGet("/", async (HttpContext context, LoanSharkDbContext db) =>
        {
            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet is null) return Results.NotFound("Wallet not found.");

            return Results.Ok(new WalletDto(wallet.Id, wallet.Balance));
        });

        group.MapGet("/transactions", async (HttpContext context, LoanSharkDbContext db) =>
        {
            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet is null) return Results.NotFound("Wallet not found.");

            var transactions = await db.LedgerTransactions
                .Where(t => t.FromWalletId == wallet.Id || t.ToWalletId == wallet.Id)
                .OrderByDescending(t => t.Timestamp)
                .Select(t => new TransactionDto(t.Id, t.Amount, t.Type.ToString(), t.Timestamp, t.FromWalletId == wallet.Id ? "Outgoing" : "Incoming"))
                .ToListAsync();

            return Results.Ok(transactions);
        });

        group.MapPost("/deposit", async (DepositDto request, HttpContext context, LoanSharkDbContext db) =>
        {
            if (request.Amount <= 0) return Results.BadRequest("Amount must be positive.");

            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet is null) return Results.NotFound("Wallet not found.");

            wallet.Balance += request.Amount;

            var transaction = new LedgerTransaction
            {
                ToWalletId = wallet.Id,
                Amount = request.Amount,
                Type = TransactionType.Deposit
            };

            db.LedgerTransactions.Add(transaction);
            await db.SaveChangesAsync();

            return Results.Ok(new WalletDto(wallet.Id, wallet.Balance));
        });

        group.MapPost("/withdraw", async (WithdrawDto request, HttpContext context, LoanSharkDbContext db) =>
        {
            if (request.Amount <= 0) return Results.BadRequest("Amount must be positive.");

            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet is null) return Results.NotFound("Wallet not found.");

            if (wallet.Balance < request.Amount) return Results.BadRequest("Insufficient funds.");

            wallet.Balance -= request.Amount;

            var transaction = new LedgerTransaction
            {
                FromWalletId = wallet.Id,
                Amount = request.Amount,
                Type = TransactionType.Withdrawal
            };

            db.LedgerTransactions.Add(transaction);
            await db.SaveChangesAsync();

            return Results.Ok(new WalletDto(wallet.Id, wallet.Balance));
        });
    }
}

public record WalletDto(Guid Id, decimal Balance);
public record TransactionDto(Guid Id, decimal Amount, string Type, DateTime Timestamp, string Direction);
public record DepositDto(decimal Amount);
public record WithdrawDto(decimal Amount);
