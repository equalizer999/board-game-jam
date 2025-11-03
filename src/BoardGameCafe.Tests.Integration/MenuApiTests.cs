using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Menu;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameCafe.Tests.Integration;

public class MenuApiTests : IClassFixture<ReservationsApiTestFixture>, IAsyncLifetime
{
    private readonly ReservationsApiTestFixture _factory;
    private HttpClient _client = null!;

    public MenuApiTests(ReservationsApiTestFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        
        // Clean up any existing data from previous tests
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        db.MenuItems.RemoveRange(db.MenuItems);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetMenuItems_WithNoItems_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/menu");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items.Should().NotBeNull();
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMenuItems_WithItems_ReturnsAllItems()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var item1 = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Meeple Mocha",
            Description = "Rich espresso with steamed milk",
            Category = MenuCategory.Coffee,
            Price = 5.50m,
            IsAvailable = true,
            PreparationTimeMinutes = 5,
            AllergenInfo = "Dairy",
            IsVegetarian = true,
            IsVegan = false,
            IsGlutenFree = true
        };

        var item2 = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Ticket to Chai",
            Description = "Spiced chai tea latte",
            Category = MenuCategory.Tea,
            Price = 4.75m,
            IsAvailable = true,
            PreparationTimeMinutes = 4,
            AllergenInfo = "Dairy",
            IsVegetarian = true,
            IsVegan = false,
            IsGlutenFree = true
        };

        db.MenuItems.AddRange(item1, item2);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/menu");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items.Should().NotBeNull();
        items.Should().HaveCount(2);
        items.Should().Contain(i => i.Name == "Meeple Mocha");
        items.Should().Contain(i => i.Name == "Ticket to Chai");
    }

    [Fact]
    public async Task GetMenuItemById_WithExistingItem_ReturnsItem()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var item = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Mana Potion IPA",
            Description = "A magical brew",
            Category = MenuCategory.Alcohol,
            Price = 7.50m,
            IsAvailable = true,
            PreparationTimeMinutes = 3,
            IsVegetarian = true,
            IsVegan = true,
            IsGlutenFree = false
        };

        db.MenuItems.Add(item);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/menu/{item.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var menuItem = await response.Content.ReadFromJsonAsync<MenuItemDto>();
        menuItem.Should().NotBeNull();
        menuItem!.Name.Should().Be("Mana Potion IPA");
        menuItem.Category.Should().Be("Alcohol");
        menuItem.Price.Should().Be(7.50m);
    }

    [Fact]
    public async Task GetMenuItemById_WithNonExistentItem_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/menu/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMenuItems_FilterByCategory_ReturnsFilteredItems()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var coffee = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Meeple Mocha",
            Description = "Coffee drink",
            Category = MenuCategory.Coffee,
            Price = 5.50m,
            IsAvailable = true,
            PreparationTimeMinutes = 5
        };

        var tea = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Ticket to Chai",
            Description = "Tea drink",
            Category = MenuCategory.Tea,
            Price = 4.75m,
            IsAvailable = true,
            PreparationTimeMinutes = 4
        };

        db.MenuItems.AddRange(coffee, tea);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/menu?category=0"); // Coffee category

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items.Should().NotBeNull();
        items.Should().HaveCount(1);
        items![0].Name.Should().Be("Meeple Mocha");
        items[0].Category.Should().Be("Coffee");
    }

    [Fact]
    public async Task GetMenuItems_FilterByVegan_ReturnsOnlyVeganItems()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var veganItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Vegan Smoothie",
            Description = "Plant-based smoothie",
            Category = MenuCategory.Snacks,
            Price = 6.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 3,
            IsVegetarian = true,
            IsVegan = true,
            IsGlutenFree = true
        };

        var nonVeganItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Cheese Pizza",
            Description = "Pizza with cheese",
            Category = MenuCategory.Meals,
            Price = 12.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 15,
            IsVegetarian = true,
            IsVegan = false,
            IsGlutenFree = false
        };

        db.MenuItems.AddRange(veganItem, nonVeganItem);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/menu?isVegan=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items.Should().NotBeNull();
        items.Should().HaveCount(1);
        items![0].Name.Should().Be("Vegan Smoothie");
        items[0].IsVegan.Should().BeTrue();
    }

    [Fact]
    public async Task GetMenuItems_FilterByPriceRange_ReturnsItemsInRange()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var cheapItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Water",
            Description = "Bottled water",
            Category = MenuCategory.Snacks,
            Price = 2.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 1
        };

        var midPriceItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Coffee",
            Description = "Regular coffee",
            Category = MenuCategory.Coffee,
            Price = 5.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 3
        };

        var expensiveItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Steak",
            Description = "Premium steak",
            Category = MenuCategory.Meals,
            Price = 25.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 30
        };

        db.MenuItems.AddRange(cheapItem, midPriceItem, expensiveItem);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/menu?minPrice=3&maxPrice=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items.Should().NotBeNull();
        items.Should().HaveCount(1);
        items![0].Name.Should().Be("Coffee");
        items[0].Price.Should().Be(5.00m);
    }

    [Fact]
    public async Task CreateMenuItem_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateMenuItemRequest
        {
            Name = "Mana Potion IPA",
            Description = "A magical brew that restores your energy",
            Category = 5, // Alcohol
            Price = 7.50m,
            PreparationTimeMinutes = 3,
            AllergenInfo = null,
            IsVegetarian = true,
            IsVegan = true,
            IsGlutenFree = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/menu", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var menuItem = await response.Content.ReadFromJsonAsync<MenuItemDto>();
        menuItem.Should().NotBeNull();
        menuItem!.Name.Should().Be("Mana Potion IPA");
        menuItem.Category.Should().Be("Alcohol");
        menuItem.Price.Should().Be(7.50m);
        menuItem.IsAvailable.Should().BeTrue(); // New items are available by default
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/v1/menu/{menuItem.Id}");
    }

    [Fact]
    public async Task CreateMenuItem_WithDuplicateName_ReturnsConflict()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var existingItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Mana Potion IPA",
            Description = "Existing item",
            Category = MenuCategory.Alcohol,
            Price = 7.50m,
            IsAvailable = true,
            PreparationTimeMinutes = 3
        };

        db.MenuItems.Add(existingItem);
        await db.SaveChangesAsync();

        var request = new CreateMenuItemRequest
        {
            Name = "Mana Potion IPA", // Duplicate name
            Description = "New item",
            Category = 5,
            Price = 8.00m,
            PreparationTimeMinutes = 3
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/menu", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateMenuItem_WithValidData_ReturnsOk()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var item = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Description = "Old description",
            Category = MenuCategory.Coffee,
            Price = 5.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 5
        };

        db.MenuItems.Add(item);
        await db.SaveChangesAsync();

        var request = new UpdateMenuItemRequest
        {
            Name = "Updated Name",
            Description = "Updated description",
            Category = 0, // Coffee
            Price = 6.00m,
            IsAvailable = false,
            PreparationTimeMinutes = 6,
            IsVegetarian = true,
            IsVegan = false,
            IsGlutenFree = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/menu/{item.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var menuItem = await response.Content.ReadFromJsonAsync<MenuItemDto>();
        menuItem.Should().NotBeNull();
        menuItem!.Name.Should().Be("Updated Name");
        menuItem.Description.Should().Be("Updated description");
        menuItem.Price.Should().Be(6.00m);
        menuItem.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateMenuItem_WithNonExistentItem_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateMenuItemRequest
        {
            Name = "Test",
            Description = "Test",
            Category = 0,
            Price = 5.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 5
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/menu/{Guid.NewGuid()}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteMenuItem_SoftDeletesItem()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var item = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "To Be Deleted",
            Description = "This will be deleted",
            Category = MenuCategory.Coffee,
            Price = 5.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 5
        };

        db.MenuItems.Add(item);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/menu/{item.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify soft delete - use a new scope to get fresh data
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        var deletedItem = await verifyDb.MenuItems.FindAsync(item.Id);
        deletedItem.Should().NotBeNull();
        deletedItem!.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteMenuItem_WithNonExistentItem_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/v1/menu/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMenuCategories_ReturnsAllCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/menu/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<object>>();
        categories.Should().NotBeNull();
        categories.Should().HaveCount(6); // Coffee, Tea, Snacks, Meals, Desserts, Alcohol
    }

    [Fact]
    public async Task GetMenuItems_FilterByAvailableOnly_ReturnsOnlyAvailableItems()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        
        var availableItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Available Item",
            Description = "This is available",
            Category = MenuCategory.Coffee,
            Price = 5.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 5
        };

        var unavailableItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Unavailable Item",
            Description = "This is not available",
            Category = MenuCategory.Coffee,
            Price = 5.00m,
            IsAvailable = false,
            PreparationTimeMinutes = 5
        };

        db.MenuItems.AddRange(availableItem, unavailableItem);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/menu?availableOnly=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items.Should().NotBeNull();
        items.Should().HaveCount(1);
        items![0].Name.Should().Be("Available Item");
        items[0].IsAvailable.Should().BeTrue();
    }
}
