using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoanShark.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using LoanShark.Api.Validation;

namespace LoanShark.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapPost("/register", async (CreateUserDto request, LoanSharkDbContext db) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
                return Results.BadRequest("Email already exists.");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), 
                Wallet = new Wallet { Balance = 0 }
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created($"/api/users/{user.Id}", new UserDto(user.Id, user.Email, user.CreatedAt));
        }).AddEndpointFilter<ValidationFilter<CreateUserDto>>();

        group.MapPost("/login", async (CreateUserDto request, LoanSharkDbContext db, IConfiguration config) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Results.BadRequest("Invalid email or password.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured."));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Results.Ok(new LoginResponseDto(tokenString, new UserDto(user.Id, user.Email, user.CreatedAt)));
        });

        group.MapGet("/{id:guid}", async (Guid id, LoanSharkDbContext db) =>
        {
            var user = await db.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == id);
            return user is not null 
                ? Results.Ok(new UserDto(user.Id, user.Email, user.CreatedAt)) 
                : Results.NotFound();
        }).RequireAuthorization();
    }
}

public record CreateUserDto(string Email, string Password);
public record UserDto(Guid Id, string Email, DateTime CreatedAt);
public record LoginResponseDto(string Token, UserDto User);
