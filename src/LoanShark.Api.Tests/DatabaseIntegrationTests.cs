using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LoanShark.Api.Entities;
using Xunit;

namespace LoanShark.Api.Tests;

public class DatabaseIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DatabaseIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        Environment.SetEnvironmentVariable("ConnectionStrings__sqldata", "Server=(localdb)\\mssqllocaldb;Database=LoanSharkTestDb;Trusted_Connection=True;MultipleActiveResultSets=true");
    }

    [Fact]
    public void DbContext_ShouldBeRegistered_And_CanConnect()
    {
        // Arrange
        // We override the connection string for testing
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:sqldata", "Server=(localdb)\\mssqllocaldb;Database=LoanSharkTestDb;Trusted_Connection=True;MultipleActiveResultSets=true"}
                });
            });
        });

        using var scope = factory.Services.CreateScope();
        
        // Act
        var dbContext = scope.ServiceProvider.GetService<LoanSharkDbContext>();

        // Assert
        Assert.NotNull(dbContext);
        
        // Create the test DB if it doesn't exist
        dbContext.Database.EnsureCreated();
        
        // Ensure the database is accessible
        var canConnect = dbContext.Database.CanConnect();
        Assert.True(canConnect);

        // Cleanup
        dbContext.Database.EnsureDeleted();
    }
}
