using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Menu;

public static class MenuEndpoints
{
    public static IEndpointRouteBuilder MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/menu")
            .WithTags("Menu");

        /// <summary>
        /// Retrieves a filtered list of menu items
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="filter">Filter parameters</param>
        /// <returns>List of menu items matching the filter criteria</returns>
        /// <response code="200">Returns the list of menu items</response>
        /// <response code="400">If the filter parameters are invalid</response>
        group.MapGet("/", async Task<Results<Ok<List<MenuItemDto>>, BadRequest<ProblemDetails>>> (
            BoardGameCafeDbContext db,
            [AsParameters] MenuFilterRequest filter) =>
        {
            var query = db.MenuItems.AsQueryable();

            // Apply category filter
            if (filter.Category.HasValue)
            {
                if (!Enum.IsDefined(typeof(MenuCategory), filter.Category.Value))
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Invalid category",
                        Detail = "Category must be a valid MenuCategory value (0-5)"
                    });
                }
                query = query.Where(m => (int)m.Category == filter.Category.Value);
            }

            // Apply dietary filters
            if (filter.IsVegetarian == true)
            {
                query = query.Where(m => m.IsVegetarian);
            }

            if (filter.IsVegan == true)
            {
                query = query.Where(m => m.IsVegan);
            }

            if (filter.IsGlutenFree == true)
            {
                query = query.Where(m => m.IsGlutenFree);
            }

            // Apply availability filter
            if (filter.AvailableOnly == true)
            {
                query = query.Where(m => m.IsAvailable);
            }

            // Apply price range filters
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(m => m.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(m => m.Price <= filter.MaxPrice.Value);
            }

            var menuItems = await query
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Category = m.Category.ToString(),
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    PreparationTimeMinutes = m.PreparationTimeMinutes,
                    AllergenInfo = m.AllergenInfo,
                    IsVegetarian = m.IsVegetarian,
                    IsVegan = m.IsVegan,
                    IsGlutenFree = m.IsGlutenFree
                })
                .ToListAsync();

            return TypedResults.Ok(menuItems);
        })
        .WithName("GetMenuItems")
        .WithSummary("List and filter menu items")
        .WithDescription("Retrieves a list of menu items with optional filters for category, dietary preferences, availability, and price range")
        .Produces<List<MenuItemDto>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Retrieves a specific menu item by its unique identifier
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="id">The unique identifier of the menu item</param>
        /// <returns>The menu item details if found</returns>
        /// <response code="200">Returns the menu item details</response>
        /// <response code="404">If the menu item is not found</response>
        group.MapGet("/{id:guid}", async Task<Results<Ok<MenuItemDto>, NotFound>> (
            BoardGameCafeDbContext db,
            Guid id) =>
        {
            var menuItem = await db.MenuItems
                .Where(m => m.Id == id)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Category = m.Category.ToString(),
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    PreparationTimeMinutes = m.PreparationTimeMinutes,
                    AllergenInfo = m.AllergenInfo,
                    IsVegetarian = m.IsVegetarian,
                    IsVegan = m.IsVegan,
                    IsGlutenFree = m.IsGlutenFree
                })
                .FirstOrDefaultAsync();

            return menuItem is null 
                ? TypedResults.NotFound() 
                : TypedResults.Ok(menuItem);
        })
        .WithName("GetMenuItemById")
        .WithSummary("Get a single menu item by ID")
        .WithDescription("Retrieves detailed information about a specific menu item")
        .Produces<MenuItemDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Creates a new menu item (Admin Only - Placeholder)
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="request">The menu item details to create</param>
        /// <returns>The created menu item details</returns>
        /// <response code="201">Returns the newly created menu item</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="409">If a menu item with the same name already exists</response>
        group.MapPost("/", async Task<Results<Created<MenuItemDto>, BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> (
            BoardGameCafeDbContext db,
            [FromBody] CreateMenuItemRequest request) =>
        {
            // Validate category is valid enum value
            if (!Enum.IsDefined(typeof(MenuCategory), request.Category))
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid category",
                    Detail = "Category must be a valid MenuCategory value (0-5)"
                });
            }

            // Check for duplicate name (case-insensitive)
            var existingItem = await db.MenuItems
                .Where(m => m.Name.ToLower() == request.Name.ToLower())
                .FirstOrDefaultAsync();

            if (existingItem is not null)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Duplicate menu item",
                    Detail = $"A menu item with the name '{request.Name}' already exists"
                });
            }

            var menuItem = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Category = (MenuCategory)request.Category,
                Price = request.Price,
                IsAvailable = true, // New items are available by default
                PreparationTimeMinutes = request.PreparationTimeMinutes,
                AllergenInfo = request.AllergenInfo,
                IsVegetarian = request.IsVegetarian,
                IsVegan = request.IsVegan,
                IsGlutenFree = request.IsGlutenFree
            };

            db.MenuItems.Add(menuItem);
            await db.SaveChangesAsync();

            var dto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Category = menuItem.Category.ToString(),
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                AllergenInfo = menuItem.AllergenInfo,
                IsVegetarian = menuItem.IsVegetarian,
                IsVegan = menuItem.IsVegan,
                IsGlutenFree = menuItem.IsGlutenFree
            };

            return TypedResults.Created($"/api/v1/menu/{menuItem.Id}", dto);
        })
        .WithName("CreateMenuItem")
        .WithSummary("Create a new menu item (Admin only - placeholder)")
        .WithDescription("Adds a new item to the menu. Currently no authentication required (admin placeholder)")
        .Produces<MenuItemDto>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        /// <summary>
        /// Updates an existing menu item (Admin Only - Placeholder)
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="id">The unique identifier of the menu item to update</param>
        /// <param name="request">The updated menu item details</param>
        /// <returns>The updated menu item details</returns>
        /// <response code="200">Returns the updated menu item</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="404">If the menu item is not found</response>
        /// <response code="409">If the update would create a duplicate name</response>
        group.MapPut("/{id:guid}", async Task<Results<Ok<MenuItemDto>, NotFound, BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> (
            BoardGameCafeDbContext db,
            Guid id,
            [FromBody] UpdateMenuItemRequest request) =>
        {
            // Validate category is valid enum value
            if (!Enum.IsDefined(typeof(MenuCategory), request.Category))
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid category",
                    Detail = "Category must be a valid MenuCategory value (0-5)"
                });
            }

            var menuItem = await db.MenuItems.FindAsync(id);
            if (menuItem is null)
            {
                return TypedResults.NotFound();
            }

            // Check for duplicate name (excluding current item, case-insensitive)
            var duplicateItem = await db.MenuItems
                .Where(m => m.Id != id && m.Name.ToLower() == request.Name.ToLower())
                .FirstOrDefaultAsync();

            if (duplicateItem is not null)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Duplicate menu item",
                    Detail = $"Another menu item with the name '{request.Name}' already exists"
                });
            }

            // Update menu item properties
            menuItem.Name = request.Name;
            menuItem.Description = request.Description;
            menuItem.Category = (MenuCategory)request.Category;
            menuItem.Price = request.Price;
            menuItem.IsAvailable = request.IsAvailable;
            menuItem.PreparationTimeMinutes = request.PreparationTimeMinutes;
            menuItem.AllergenInfo = request.AllergenInfo;
            menuItem.IsVegetarian = request.IsVegetarian;
            menuItem.IsVegan = request.IsVegan;
            menuItem.IsGlutenFree = request.IsGlutenFree;

            await db.SaveChangesAsync();

            var dto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Category = menuItem.Category.ToString(),
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                AllergenInfo = menuItem.AllergenInfo,
                IsVegetarian = menuItem.IsVegetarian,
                IsVegan = menuItem.IsVegan,
                IsGlutenFree = menuItem.IsGlutenFree
            };

            return TypedResults.Ok(dto);
        })
        .WithName("UpdateMenuItem")
        .WithSummary("Update an existing menu item (Admin only - placeholder)")
        .WithDescription("Updates the details of an existing menu item. Currently no authentication required (admin placeholder)")
        .Produces<MenuItemDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        /// <summary>
        /// Soft deletes a menu item by setting IsAvailable to false (Admin Only - Placeholder)
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="id">The unique identifier of the menu item to delete</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Menu item successfully marked as unavailable</response>
        /// <response code="404">If the menu item is not found</response>
        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (
            BoardGameCafeDbContext db,
            Guid id) =>
        {
            var menuItem = await db.MenuItems.FindAsync(id);
            if (menuItem is null)
            {
                return TypedResults.NotFound();
            }

            // Soft delete: set IsAvailable to false
            menuItem.IsAvailable = false;
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithName("DeleteMenuItem")
        .WithSummary("Soft delete a menu item (Admin only - placeholder)")
        .WithDescription("Marks a menu item as unavailable (soft delete). Currently no authentication required (admin placeholder)")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Retrieves all available menu categories
        /// </summary>
        /// <returns>List of all menu categories</returns>
        /// <response code="200">Returns the list of categories</response>
        group.MapGet("/categories", () =>
        {
            var categories = Enum.GetValues<MenuCategory>()
                .Select(c => new
                {
                    Value = (int)c,
                    Name = c.ToString()
                })
                .ToList();

            return TypedResults.Ok(categories);
        })
        .WithName("GetMenuCategories")
        .WithSummary("List all menu categories")
        .WithDescription("Retrieves all available menu categories (Coffee, Tea, Snacks, Meals, Desserts, Alcohol)")
        .Produces<List<object>>(StatusCodes.Status200OK);

        return app;
    }
}
