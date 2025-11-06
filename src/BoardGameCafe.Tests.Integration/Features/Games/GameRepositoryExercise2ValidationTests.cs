using BoardGameCafe.Api.Features.Games;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameCafe.Tests.Integration.Features.Games;

/// <summary>
/// Validates that Exercise 2 patterns work correctly with the repository
/// </summary>
public class GameRepositoryExercise2ValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GameRepositoryExercise2ValidationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void IGameRepository_IsRegisteredInDI()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        
        // Act
        var repository = scope.ServiceProvider.GetService<IGameRepository>();
        
        // Assert
        repository.Should().NotBeNull("IGameRepository must be registered in DI for Exercise 2");
        repository.Should().BeOfType<GameRepository>("GameRepository should be the implementation");
    }
}
