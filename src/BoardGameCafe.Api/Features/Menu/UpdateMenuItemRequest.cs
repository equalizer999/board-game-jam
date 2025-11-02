using System.ComponentModel.DataAnnotations;

namespace BoardGameCafe.Api.Features.Menu;

/// <summary>
/// Request to update an existing menu item
/// </summary>
public record UpdateMenuItemRequest
{
    /// <summary>
    /// Name of the menu item
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Description of the menu item
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// Category as integer (0=Coffee, 1=Tea, 2=Snacks, 3=Meals, 4=Desserts, 5=Alcohol)
    /// </summary>
    [Required]
    [Range(0, 5)]
    public int Category { get; init; }
    
    /// <summary>
    /// Price of the menu item
    /// </summary>
    [Required]
    [Range(0.01, 999.99)]
    public decimal Price { get; init; }
    
    /// <summary>
    /// Whether the item is currently available for ordering
    /// </summary>
    public bool IsAvailable { get; init; }
    
    /// <summary>
    /// Estimated preparation time in minutes
    /// </summary>
    [Required]
    [Range(1, 120)]
    public int PreparationTimeMinutes { get; init; }
    
    /// <summary>
    /// Allergen information (e.g., "Dairy, Nuts, Gluten")
    /// </summary>
    [MaxLength(200)]
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
