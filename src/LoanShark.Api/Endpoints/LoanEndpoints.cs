using System.Security.Claims;
using LoanShark.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanShark.Api.Endpoints;

public static class LoanEndpoints
{
    public static void MapLoanEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/loans").WithTags("Loans").RequireAuthorization();

        group.MapPost("/", async (CreateLoanDto request, HttpContext context, LoanSharkDbContext db) =>
        {
            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var borrower = await db.Users.FindAsync(userId);
            if (borrower is null) return Results.NotFound("Borrower not found.");

            var loan = new LoanRequest
            {
                BorrowerId = userId,
                AmountRequested = request.Amount,
                InterestRate = request.InterestRate,
                TermDays = request.TermDays,
                Status = LoanStatus.Pending
            };

            db.LoanRequests.Add(loan);
            await db.SaveChangesAsync();

            return Results.Created($"/api/loans/{loan.Id}", new LoanDto(loan.Id, loan.BorrowerId, loan.AmountRequested, loan.AmountFunded, loan.InterestRate, loan.TermDays, loan.Status.ToString()));
        });

        group.MapGet("/", async (LoanSharkDbContext db) =>
        {
            var loans = await db.LoanRequests
                .Where(l => l.Status == LoanStatus.Pending || l.Status == LoanStatus.Active)
                .Select(l => new LoanDto(l.Id, l.BorrowerId, l.AmountRequested, l.AmountFunded, l.InterestRate, l.TermDays, l.Status.ToString()))
                .ToListAsync();
            return Results.Ok(loans);
        });

        group.MapGet("/{id:guid}", async (Guid id, LoanSharkDbContext db) =>
        {
            var loan = await db.LoanRequests.FindAsync(id);
            if (loan is null) return Results.NotFound();

            return Results.Ok(new LoanDto(loan.Id, loan.BorrowerId, loan.AmountRequested, loan.AmountFunded, loan.InterestRate, loan.TermDays, loan.Status.ToString()));
        });

        group.MapPost("/{id:guid}/fund", async (Guid id, FundLoanDto request, HttpContext context, LoanSharkDbContext db) =>
        {
            var lenderIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(lenderIdStr, out var lenderId)) return Results.Unauthorized();

            var loan = await db.LoanRequests.Include(l => l.Investments).FirstOrDefaultAsync(l => l.Id == id);
            if (loan is null) return Results.NotFound("Loan not found.");

            if (loan.Status != LoanStatus.Pending) return Results.BadRequest("Loan is not pending funding.");
            
            if (loan.BorrowerId == lenderId) return Results.BadRequest("You cannot fund your own loan.");

            var amountRemaining = loan.AmountRequested - loan.AmountFunded;
            if (request.Amount > amountRemaining) return Results.BadRequest($"Cannot fund more than the remaining amount: {amountRemaining}");

            var lenderWallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == lenderId);
            if (lenderWallet is null) return Results.NotFound("Lender wallet not found.");

            if (lenderWallet.Balance < request.Amount) return Results.BadRequest("Insufficient funds in wallet.");

            var borrowerWallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == loan.BorrowerId);
            if (borrowerWallet is null) return Results.NotFound("Borrower wallet not found.");

            var investment = new LoanInvestment
            {
                LoanRequestId = loan.Id,
                LenderId = lenderId,
                Amount = request.Amount
            };

            loan.Investments.Add(investment);
            loan.AmountFunded += request.Amount;

            // Deduct from lender, add to borrower (in a more complex system, this might go to an escrow state first)
            lenderWallet.Balance -= request.Amount;
            borrowerWallet.Balance += request.Amount;

            // Record transaction
            var transaction = new LedgerTransaction
            {
                FromWalletId = lenderWallet.Id,
                ToWalletId = borrowerWallet.Id,
                Amount = request.Amount,
                Type = TransactionType.LoanFunding,
                ReferenceId = loan.Id
            };
            db.LedgerTransactions.Add(transaction);

            if (loan.AmountFunded >= loan.AmountRequested)
            {
                loan.Status = LoanStatus.FullyFunded;
            }

            await db.SaveChangesAsync();

            return Results.Ok(new LoanDto(loan.Id, loan.BorrowerId, loan.AmountRequested, loan.AmountFunded, loan.InterestRate, loan.TermDays, loan.Status.ToString()));
        });
    }
}

public record CreateLoanDto(decimal Amount, decimal InterestRate, int TermDays);
public record FundLoanDto(decimal Amount);
public record LoanDto(Guid Id, Guid BorrowerId, decimal AmountRequested, decimal AmountFunded, decimal InterestRate, int TermDays, string Status);
