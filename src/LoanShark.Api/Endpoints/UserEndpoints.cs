using LoanShark.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanShark.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapPost("/", async (CreateUserDto request, LoanSharkDbContext db) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
                return Results.BadRequest("Email already exists.");

            var user = new User
            {
                Email = request.Email,
                // TODO: Hash password properly in a future iteration
                PasswordHash = request.Password, 
                Wallet = new Wallet { Balance = 0 }
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created($"/api/users/{user.Id}", new UserDto(user.Id, user.Email, user.CreatedAt));
        });

        group.MapGet("/{id:guid}", async (Guid id, LoanSharkDbContext db) =>
        {
            var user = await db.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == id);
            return user is not null 
                ? Results.Ok(new UserDto(user.Id, user.Email, user.CreatedAt)) 
                : Results.NotFound();
        });
    }
}

public record CreateUserDto(string Email, string Password);
public record UserDto(Guid Id, string Email, DateTime CreatedAt);
