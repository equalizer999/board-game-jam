using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Orders;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/orders")
            .WithTags("Orders");

        // GET /api/v1/orders?customerId={id}
        group.MapGet("/", GetOrders)
            .WithName("GetOrders")
            .WithSummary("List all orders for a customer")
            .Produces<List<OrderDto>>(StatusCodes.Status200OK);

        // GET /api/v1/orders/{id}
        group.MapGet("/{id:guid}", GetOrder)
            .WithName("GetOrder")
            .WithSummary("Get a single order with all items")
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/orders
        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithSummary("Create a new draft order")
            .Produces<OrderDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // POST /api/v1/orders/{id}/items
        group.MapPost("/{id:guid}/items", AddOrderItem)
            .WithName("AddOrderItem")
            .WithSummary("Add an item to a draft order")
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // DELETE /api/v1/orders/{id}/items/{itemId}
        group.MapDelete("/{id:guid}/items/{itemId:guid}", RemoveOrderItem)
            .WithName("RemoveOrderItem")
            .WithSummary("Remove an item from a draft order")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // PATCH /api/v1/orders/{id}
        group.MapPatch("/{id:guid}", UpdateOrder)
            .WithName("UpdateOrder")
            .WithSummary("Update item quantities in a draft order")
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/orders/{id}/submit
        group.MapPost("/{id:guid}/submit", SubmitOrder)
            .WithName("SubmitOrder")
            .WithSummary("Finalize a draft order and apply calculations")
            .Produces<OrderSummaryDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/orders/{id}/pay
        group.MapPost("/{id:guid}/pay", PayOrder)
            .WithName("PayOrder")
            .WithSummary("Process payment for a submitted order")
            .Produces<OrderSummaryDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// List all orders for a specific customer
    /// </summary>
    private static async Task<Ok<List<OrderDto>>> GetOrders(
        BoardGameCafeDbContext db,
        Guid customerId,
        CancellationToken ct)
    {
        var orders = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(ct);

        var orderDtos = orders.Select(MapToOrderDto).ToList();
        return TypedResults.Ok(orderDtos);
    }

    /// <summary>
    /// Get a single order by ID with all items
    /// </summary>
    private static async Task<Results<Ok<OrderDto>, NotFound>> GetOrder(
        BoardGameCafeDbContext db,
        Guid id,
        CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(MapToOrderDto(order));
    }

    /// <summary>
    /// Create a new draft order
    /// </summary>
    private static async Task<Results<Created<OrderDto>, BadRequest<ProblemDetails>>> CreateOrder(
        BoardGameCafeDbContext db,
        CreateOrderRequest request,
        CancellationToken ct)
    {
        // Validate customer exists
        var customer = await db.Customers.FindAsync([request.CustomerId], ct);
        if (customer == null)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid customer",
                Detail = "Customer not found"
            });
        }

        // Validate reservation if provided
        if (request.ReservationId.HasValue)
        {
            var reservation = await db.Reservations.FindAsync([request.ReservationId.Value], ct);
            if (reservation == null)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid reservation",
                    Detail = "Reservation not found"
                });
            }
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            ReservationId = request.ReservationId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Draft,
            PaymentMethod = PaymentMethod.Cash
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        // Reload with customer for DTO mapping
        order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .FirstAsync(o => o.Id == order.Id, ct);

        var dto = MapToOrderDto(order);
        return TypedResults.Created($"/api/v1/orders/{order.Id}", dto);
    }

    /// <summary>
    /// Add an item to a draft order
    /// </summary>
    private static async Task<Results<Ok<OrderDto>, NotFound, BadRequest<ProblemDetails>>> AddOrderItem(
        BoardGameCafeDbContext db,
        Guid id,
        AddOrderItemRequest request,
        CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
        {
            return TypedResults.NotFound();
        }

        // Validate order is in draft status
        if (order.Status != OrderStatus.Draft)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order not modifiable",
                Detail = "Only draft orders can be modified"
            });
        }

        // Validate quantity
        if (request.Quantity <= 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid quantity",
                Detail = "Quantity must be greater than 0"
            });
        }

        // Validate menu item exists and is available
        var menuItem = await db.MenuItems.FindAsync([request.MenuItemId], ct);
        if (menuItem == null)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid menu item",
                Detail = "Menu item not found"
            });
        }

        if (!menuItem.IsAvailable)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Menu item unavailable",
                Detail = $"{menuItem.Name} is currently unavailable"
            });
        }

        // Check if item already exists in order
        var existingItem = order.Items.FirstOrDefault(i => i.MenuItemId == request.MenuItemId);
        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            // Add new item with current price snapshot
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                MenuItemId = request.MenuItemId,
                Quantity = request.Quantity,
                UnitPrice = menuItem.Price,
                SpecialInstructions = request.SpecialInstructions
            };
            order.Items.Add(orderItem);
            db.OrderItems.Add(orderItem);
        }

        // Recalculate order totals
        order.RecalculateTotals();

        await db.SaveChangesAsync(ct);

        // Reload to get menu item names
        order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstAsync(o => o.Id == id, ct);

        return TypedResults.Ok(MapToOrderDto(order));
    }

    /// <summary>
    /// Remove an item from a draft order
    /// </summary>
    private static async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> RemoveOrderItem(
        BoardGameCafeDbContext db,
        Guid id,
        Guid itemId,
        CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
        {
            return TypedResults.NotFound();
        }

        // Validate order is in draft status
        if (order.Status != OrderStatus.Draft)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order not modifiable",
                Detail = "Only draft orders can be modified"
            });
        }

        var item = order.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return TypedResults.NotFound();
        }

        order.Items.Remove(item);
        db.OrderItems.Remove(item);

        // Recalculate order totals
        order.RecalculateTotals();

        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Update item quantities in a draft order
    /// </summary>
    private static async Task<Results<Ok<OrderDto>, NotFound, BadRequest<ProblemDetails>>> UpdateOrder(
        BoardGameCafeDbContext db,
        Guid id,
        UpdateOrderRequest request,
        CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
        {
            return TypedResults.NotFound();
        }

        // Validate order is in draft status
        if (order.Status != OrderStatus.Draft)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order not modifiable",
                Detail = "Only draft orders can be modified"
            });
        }

        // Update quantities
        foreach (var update in request.ItemUpdates)
        {
            var item = order.Items.FirstOrDefault(i => i.Id == update.OrderItemId);
            if (item == null)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid item",
                    Detail = $"Order item {update.OrderItemId} not found"
                });
            }

            if (update.Quantity < 0)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid quantity",
                    Detail = "Quantity cannot be negative"
                });
            }

            if (update.Quantity == 0)
            {
                // Remove item
                order.Items.Remove(item);
                db.OrderItems.Remove(item);
            }
            else
            {
                // Update quantity
                item.Quantity = update.Quantity;
            }
        }

        // Recalculate order totals
        order.RecalculateTotals();

        await db.SaveChangesAsync(ct);

        // Reload to ensure all data is fresh
        order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstAsync(o => o.Id == id, ct);

        return TypedResults.Ok(MapToOrderDto(order));
    }

    /// <summary>
    /// Finalize a draft order and apply calculations
    /// </summary>
    private static async Task<Results<Ok<OrderSummaryDto>, NotFound, BadRequest<ProblemDetails>>> SubmitOrder(
        BoardGameCafeDbContext db,
        OrderCalculationService calculationService,
        Guid id,
        int loyaltyPointsToRedeem = 0,
        CancellationToken ct = default)
    {
        var order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
        {
            return TypedResults.NotFound();
        }

        // Validate order is in draft status
        if (order.Status != OrderStatus.Draft)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order already submitted",
                Detail = "Order has already been submitted"
            });
        }

        // Validate order has items
        if (!order.Items.Any())
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Empty order",
                Detail = "Cannot submit an order with no items"
            });
        }

        // Validate loyalty points redemption
        if (loyaltyPointsToRedeem > 0)
        {
            if (!calculationService.ValidateLoyaltyPointsRedemption(order.Customer, loyaltyPointsToRedeem))
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Insufficient loyalty points",
                    Detail = $"Customer has {order.Customer.LoyaltyPoints} points, cannot redeem {loyaltyPointsToRedeem}"
                });
            }
            
            // Deduct redeemed loyalty points from customer
            order.Customer.LoyaltyPoints -= loyaltyPointsToRedeem;
            
            // Track loyalty points redemption
            var redemptionHistory = new LoyaltyPointsHistory
            {
                Id = Guid.NewGuid(),
                CustomerId = order.CustomerId,
                OrderId = order.Id,
                PointsChange = -loyaltyPointsToRedeem,
                TransactionType = LoyaltyTransactionType.Redeemed,
                TransactionDate = DateTime.UtcNow,
                Description = $"Redeemed {loyaltyPointsToRedeem} points on order"
            };
            db.LoyaltyPointsHistory.Add(redemptionHistory);
        }

        // Calculate order totals including loyalty points redemption
        calculationService.CalculateOrderTotals(order, loyaltyPointsToRedeem);

        // Update order status
        order.Status = OrderStatus.Submitted;

        await db.SaveChangesAsync(ct);

        var summary = MapToOrderSummaryDto(order, calculationService, loyaltyPointsToRedeem);
        return TypedResults.Ok(summary);
    }

    /// <summary>
    /// Process payment for a submitted order
    /// </summary>
    private static async Task<Results<Ok<OrderSummaryDto>, NotFound, BadRequest<ProblemDetails>>> PayOrder(
        BoardGameCafeDbContext db,
        OrderCalculationService calculationService,
        Guid id,
        PaymentMethod paymentMethod,
        CancellationToken ct = default)
    {
        var order = await db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
        {
            return TypedResults.NotFound();
        }

        // Validate order is submitted
        if (order.Status != OrderStatus.Submitted)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order not submitted",
                Detail = "Order must be submitted before payment"
            });
        }

        // Calculate loyalty points earned from the final order total
        var pointsEarned = calculationService.CalculateLoyaltyPointsEarned(order.TotalAmount);

        // Update customer loyalty points
        order.Customer.LoyaltyPoints += pointsEarned;
        
        // Track loyalty points earned
        if (pointsEarned > 0)
        {
            var earnedHistory = new LoyaltyPointsHistory
            {
                Id = Guid.NewGuid(),
                CustomerId = order.CustomerId,
                OrderId = order.Id,
                PointsChange = pointsEarned,
                TransactionType = LoyaltyTransactionType.Earned,
                TransactionDate = DateTime.UtcNow,
                Description = $"Earned {pointsEarned} points from order (${order.TotalAmount:F2})"
            };
            db.LoyaltyPointsHistory.Add(earnedHistory);
        }

        // Update payment method and status
        order.PaymentMethod = paymentMethod;
        order.Status = OrderStatus.Completed;

        await db.SaveChangesAsync(ct);

        // Note: loyaltyPointsRedeemed is 0 here because redemption happens during submit
        var summary = MapToOrderSummaryDto(order, calculationService, 0);
        return TypedResults.Ok(summary);
    }

    private static OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ReservationId = order.ReservationId,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            PaymentMethod = order.PaymentMethod.ToString(),
            CustomerName = $"{order.Customer.FirstName} {order.Customer.LastName}",
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItem?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                ItemTotal = i.Quantity * i.UnitPrice,
                SpecialInstructions = i.SpecialInstructions
            }).ToList()
        };
    }

    private static OrderSummaryDto MapToOrderSummaryDto(Order order, OrderCalculationService calculationService, int loyaltyPointsRedeemed)
    {
        var memberDiscountPercentage = calculationService.GetMemberDiscountPercentage(order.Customer.MembershipTier);
        var memberDiscountAmount = order.Subtotal * memberDiscountPercentage;
        var loyaltyDiscountAmount = calculationService.CalculateLoyaltyPointsDiscount(loyaltyPointsRedeemed);
        var pointsEarned = calculationService.CalculateLoyaltyPointsEarned(order.TotalAmount);

        return new OrderSummaryDto
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = $"{order.Customer.FirstName} {order.Customer.LastName}",
            MembershipTier = order.Customer.MembershipTier.ToString(),
            LoyaltyPointsBalance = order.Customer.LoyaltyPoints,
            Subtotal = order.Subtotal,
            MemberDiscountAmount = memberDiscountAmount,
            MemberDiscountPercentage = memberDiscountPercentage * 100, // Convert to percentage
            LoyaltyPointsRedeemed = loyaltyPointsRedeemed,
            LoyaltyPointsDiscountAmount = loyaltyDiscountAmount,
            TotalDiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            LoyaltyPointsEarned = pointsEarned,
            Status = order.Status.ToString(),
            PaymentMethod = order.PaymentMethod.ToString(),
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItem?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                ItemTotal = i.Quantity * i.UnitPrice,
                SpecialInstructions = i.SpecialInstructions
            }).ToList()
        };
    }
}
