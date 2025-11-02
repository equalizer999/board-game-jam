using BoardGameCafe.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new BoardGameCafeDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<BoardGameCafeDbContext>>());

        // Check if already seeded
        if (context.MenuItems.Any())
        {
            return; // Database has been seeded
        }

        SeedMenuItems(context);
        SeedSampleOrders(context);

        context.SaveChanges();
    }

    private static void SeedMenuItems(BoardGameCafeDbContext context)
    {
        var menuItems = new List<MenuItem>
        {
            // Coffee Items
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Meeple Mocha",
                Description = "Rich espresso with steamed milk and chocolate, topped with whipped cream and a chocolate meeple",
                Category = MenuCategory.Coffee,
                Price = 5.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Catan Cappuccino",
                Description = "Classic cappuccino with perfectly steamed foam, served in our custom hexagonal cup",
                Category = MenuCategory.Coffee,
                Price = 4.75m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Espresso Shot",
                Description = "Double shot of our house espresso blend",
                Category = MenuCategory.Coffee,
                Price = 3.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Azul Americano",
                Description = "Smooth espresso diluted with hot water",
                Category = MenuCategory.Coffee,
                Price = 3.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Root Beer Latte",
                Description = "Unique latte with house-made root beer syrup",
                Category = MenuCategory.Coffee,
                Price = 5.25m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },

            // Tea Items
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Ticket to Chai",
                Description = "Aromatic chai tea with warm spices and steamed milk",
                Category = MenuCategory.Tea,
                Price = 4.25m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Earl Grey Supremacy",
                Description = "Premium Earl Grey tea with bergamot essence",
                Category = MenuCategory.Tea,
                Price = 3.75m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Green Tea of Carcassonne",
                Description = "Delicate Japanese green tea",
                Category = MenuCategory.Tea,
                Price = 3.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },

            // Snacks
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Game Night Nachos",
                Description = "Crispy tortilla chips with melted cheese, jalapeños, sour cream, and salsa",
                Category = MenuCategory.Snacks,
                Price = 8.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 10,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Dice Tower Fries",
                Description = "Crispy seasoned fries stacked high with dipping sauces",
                Category = MenuCategory.Snacks,
                Price = 6.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 8,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Meeple Mix",
                Description = "Assorted nuts, dried fruits, and chocolate pieces",
                Category = MenuCategory.Snacks,
                Price = 5.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 1,
                AllergenInfo = "Nuts, Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },

            // Meals
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Pandemic Pizza",
                Description = "12-inch pizza with your choice of toppings, named after the game that brought the world together",
                Category = MenuCategory.Meals,
                Price = 14.99m,
                IsAvailable = true,
                PreparationTimeMinutes = 20,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Wingspan Wings",
                Description = "Crispy chicken wings tossed in your choice of sauce",
                Category = MenuCategory.Meals,
                Price = 12.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 18,
                AllergenInfo = "Gluten, Soy",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Settlers Sandwich",
                Description = "Grilled chicken sandwich with lettuce, tomato, and house sauce on artisan bread",
                Category = MenuCategory.Meals,
                Price = 11.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 15,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Carcassonne Cobb Salad",
                Description = "Fresh mixed greens with chicken, bacon, eggs, avocado, and blue cheese",
                Category = MenuCategory.Meals,
                Price = 13.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 12,
                AllergenInfo = "Dairy, Eggs",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Vegan Victory Bowl",
                Description = "Quinoa bowl with roasted vegetables, chickpeas, and tahini dressing",
                Category = MenuCategory.Meals,
                Price = 12.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 15,
                AllergenInfo = "Sesame",
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },

            // Desserts
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Chocolate Chip Cookie",
                Description = "Warm, fresh-baked chocolate chip cookie",
                Category = MenuCategory.Desserts,
                Price = 3.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Kingdomino Brownie",
                Description = "Rich chocolate brownie topped with vanilla ice cream",
                Category = MenuCategory.Desserts,
                Price = 6.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Game Over Cheesecake",
                Description = "Creamy New York style cheesecake with seasonal fruit topping",
                Category = MenuCategory.Desserts,
                Price = 7.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },

            // Alcohol
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Board Game Brew IPA",
                Description = "Local craft IPA with citrus notes",
                Category = MenuCategory.Alcohol,
                Price = 7.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Gluten",
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Red Wine - Catan Reserve",
                Description = "Medium-bodied red wine with berry notes",
                Category = MenuCategory.Alcohol,
                Price = 9.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Sulfites",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Rolling Dice Cider",
                Description = "Crisp apple cider, refreshing and slightly sweet",
                Category = MenuCategory.Alcohol,
                Price = 6.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            }
        };

        context.MenuItems.AddRange(menuItems);
    }

    private static void SeedSampleOrders(BoardGameCafeDbContext context)
    {
        // Only seed orders if we have customers
        if (!context.Customers.Any())
            return;

        var customer = context.Customers.First();

        // Sample Order 1: Coffee and snacks
        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow.AddDays(-5),
            Status = OrderStatus.Completed,
            PaymentMethod = PaymentMethod.Card,
            Items = new List<OrderItem>()
        };

        var meepleMocha = context.MenuItems.FirstOrDefault(m => m.Name == "Meeple Mocha");
        var nachos = context.MenuItems.FirstOrDefault(m => m.Name == "Game Night Nachos");

        if (meepleMocha != null)
        {
            order1.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = meepleMocha.Id,
                Quantity = 2,
                UnitPrice = meepleMocha.Price,
                SpecialInstructions = "Extra chocolate please"
            });
        }

        if (nachos != null)
        {
            order1.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = nachos.Id,
                Quantity = 1,
                UnitPrice = nachos.Price,
                SpecialInstructions = null
            });
        }

        context.Orders.Add(order1);

        // Sample Order 2: Full meal with drinks
        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow.AddDays(-2),
            Status = OrderStatus.Completed,
            PaymentMethod = PaymentMethod.Card,
            Items = new List<OrderItem>()
        };

        var pizza = context.MenuItems.FirstOrDefault(m => m.Name == "Pandemic Pizza");
        var wings = context.MenuItems.FirstOrDefault(m => m.Name == "Wingspan Wings");
        var beer = context.MenuItems.FirstOrDefault(m => m.Name == "Board Game Brew IPA");

        if (pizza != null)
        {
            order2.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = pizza.Id,
                Quantity = 1,
                UnitPrice = pizza.Price
            });
        }

        if (wings != null)
        {
            order2.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = wings.Id,
                Quantity = 1,
                UnitPrice = wings.Price,
                SpecialInstructions = "Buffalo sauce"
            });
        }

        if (beer != null)
        {
            order2.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = beer.Id,
                Quantity = 2,
                UnitPrice = beer.Price
            });
        }

        context.Orders.Add(order2);
    }
}
