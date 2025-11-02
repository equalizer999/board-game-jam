namespace BoardGameCafe.Api.Features.Menu;

/// <summary>
/// Represents a menu item with all details
/// </summary>
public record MenuItemDto
{
    /// <summary>
    /// Unique identifier for the menu item
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Name of the menu item
    /// </summary>
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Description of the menu item
    /// </summary>
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// Category of the menu item (Coffee, Tea, Snacks, Meals, Desserts, Alcohol)
    /// </summary>
    public string Category { get; init; } = string.Empty;
    
    /// <summary>
    /// Price of the menu item
    /// </summary>
    public decimal Price { get; init; }
    
    /// <summary>
    /// Whether the item is currently available for ordering
    /// </summary>
    public bool IsAvailable { get; init; }
    
    /// <summary>
    /// Estimated preparation time in minutes
    /// </summary>
    public int PreparationTimeMinutes { get; init; }
    
    /// <summary>
    /// Allergen information (e.g., "Dairy, Nuts, Gluten")
    /// </summary>
    public string? AllergenInfo { get; init; }
    
    /// <summary>
    /// Whether the item is suitable for vegetarians
    /// </summary>
    public bool IsVegetarian { get; init; }
    
    /// <summary>
    /// Whether the item is suitable for vegans
    /// </summary>
    public bool IsVegan { get; init; }
    
    /// <summary>
    /// Whether the item is gluten-free
    /// </summary>
    public bool IsGlutenFree { get; init; }
}
