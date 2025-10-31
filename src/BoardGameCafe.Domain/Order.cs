namespace BoardGameCafe.Domain;

public class Order
{
    public Guid Id { get; set; }
    public Guid? ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// Calculates the subtotal from all order items
    /// </summary>
    public void CalculateSubtotal()
    {
        Subtotal = Items.Sum(item => item.Quantity * item.UnitPrice);
    }

    /// <summary>
    /// Calculates tax based on item categories
    /// Tax rate: 8% on food, 10% on alcohol
    /// </summary>
    public void CalculateTax()
    {
        decimal foodTax = 0;
        decimal alcoholTax = 0;

        foreach (var item in Items)
        {
            if (item.MenuItem == null) continue;

            var itemTotal = item.Quantity * item.UnitPrice;
            if (item.MenuItem.Category == MenuCategory.Alcohol)
            {
                alcoholTax += itemTotal * 0.10m;
            }
            else
            {
                foodTax += itemTotal * 0.08m;
            }
        }

        TaxAmount = foodTax + alcoholTax;
    }

    /// <summary>
    /// Calculates member discount based on customer's membership tier
    /// Bronze: 5%, Silver: 10%, Gold: 15%
    /// </summary>
    public void CalculateMemberDiscount()
    {
        if (Customer == null) return;

        var discountRate = Customer.MembershipTier switch
        {
            MembershipTier.Bronze => 0.05m,
            MembershipTier.Silver => 0.10m,
            MembershipTier.Gold => 0.15m,
            _ => 0m
        };

        DiscountAmount = Subtotal * discountRate;
    }

    /// <summary>
    /// Calculates the total amount: Subtotal - Discount + Tax
    /// </summary>
    public void CalculateTotal()
    {
        TotalAmount = Subtotal - DiscountAmount + TaxAmount;
    }

    /// <summary>
    /// Recalculates all order totals (subtotal, discount, tax, and total)
    /// Call this after modifying order items or customer
    /// </summary>
    public void RecalculateTotals()
    {
        CalculateSubtotal();
        CalculateMemberDiscount();
        CalculateTax();
        CalculateTotal();
    }
}

public enum OrderStatus
{
    Draft,
    Submitted,
    InProgress,
    Ready,
    Delivered,
    Completed,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Card,
    LoyaltyPoints
}
