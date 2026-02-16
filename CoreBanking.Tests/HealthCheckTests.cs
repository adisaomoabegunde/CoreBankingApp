using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoreBanking.Tests;

public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthCheckTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Api_StartupSucceeds()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act & Assert - verifies the app starts without exceptions
        Assert.NotNull(client);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
