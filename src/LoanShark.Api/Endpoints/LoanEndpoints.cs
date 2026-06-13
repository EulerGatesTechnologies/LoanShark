using LoanShark.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanShark.Api.Endpoints;

public static class LoanEndpoints
{
    public static void MapLoanEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/loans").WithTags("Loans");

        group.MapPost("/", async (CreateLoanDto request, LoanSharkDbContext db) =>
        {
            var borrower = await db.Users.FindAsync(request.BorrowerId);
            if (borrower is null) return Results.NotFound("Borrower not found.");

            var loan = new LoanRequest
            {
                BorrowerId = request.BorrowerId,
                AmountRequested = request.Amount,
                InterestRate = request.InterestRate,
                TermDays = request.TermDays
            };

            db.LoanRequests.Add(loan);
            await db.SaveChangesAsync();

            return Results.Created($"/api/loans/{loan.Id}", loan);
        });

        group.MapGet("/", async (LoanSharkDbContext db) =>
        {
            var loans = await db.LoanRequests
                .Where(l => l.Status == LoanStatus.Pending)
                .ToListAsync();
            return Results.Ok(loans);
        });
    }
}

public record CreateLoanDto(Guid BorrowerId, decimal Amount, decimal InterestRate, int TermDays);
