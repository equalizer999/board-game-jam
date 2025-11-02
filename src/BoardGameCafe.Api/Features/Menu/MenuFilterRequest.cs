using Microsoft.AspNetCore.Mvc;

namespace BoardGameCafe.Api.Features.Menu;

/// <summary>
/// Query parameters for filtering menu items
/// </summary>
public record MenuFilterRequest
{
    /// <summary>
    /// Filter by category (0=Coffee, 1=Tea, 2=Snacks, 3=Meals, 4=Desserts, 5=Alcohol)
    /// </summary>
    [FromQuery(Name = "category")]
    public int? Category { get; init; }
    
    /// <summary>
    /// Filter by vegetarian items only
    /// </summary>
    [FromQuery(Name = "isVegetarian")]
    public bool? IsVegetarian { get; init; }
    
    /// <summary>
    /// Filter by vegan items only
    /// </summary>
    [FromQuery(Name = "isVegan")]
    public bool? IsVegan { get; init; }
    
    /// <summary>
    /// Filter by gluten-free items only
    /// </summary>
    [FromQuery(Name = "isGlutenFree")]
    public bool? IsGlutenFree { get; init; }
    
    /// <summary>
    /// Filter by available items only
    /// </summary>
    [FromQuery(Name = "availableOnly")]
    public bool? AvailableOnly { get; init; }
    
    /// <summary>
    /// Minimum price filter
    /// </summary>
    [FromQuery(Name = "minPrice")]
    public decimal? MinPrice { get; init; }
    
    /// <summary>
    /// Maximum price filter
    /// </summary>
    [FromQuery(Name = "maxPrice")]
    public decimal? MaxPrice { get; init; }
}
