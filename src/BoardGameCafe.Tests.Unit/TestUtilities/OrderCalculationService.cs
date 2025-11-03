using BoardGameCafe.Domain;

namespace BoardGameCafe.Tests.Unit.TestUtilities;

/// <summary>
/// Service for calculating order totals, discounts, taxes, and loyalty points
/// </summary>
public class OrderCalculationService
{
    private const decimal LoyaltyPointsToDiscountRate = 0.01m; // 100 points = $1
    private const decimal FoodTaxRate = 0.08m; // 8%
    private const decimal AlcoholTaxRate = 0.10m; // 10%

    /// <summary>
    /// Calculates all order totals including discounts, tax, and loyalty points.
    /// 
    /// Calculation order:
    /// 1. Subtotal = sum of (quantity * unitPrice) for all items
    /// 2. Member discount = subtotal * membership tier percentage
    /// 3. Loyalty points discount = loyaltyPointsToRedeem * $0.01
    /// 4. Tax = calculated on original item prices (not discounted amount), based on category
    /// 5. Total = subtotal - total discounts + tax
    /// 6. Negative total prevention = reduces discount if total would be negative
    /// 
    /// Note: Tax is calculated on the original item prices before discounts are applied.
    /// This is a business requirement to ensure proper tax reporting.
    /// </summary>
    /// <param name="order">The order to calculate</param>
    /// <param name="loyaltyPointsToRedeem">Number of loyalty points to redeem (optional)</param>
    public void CalculateOrderTotals(Order order, int loyaltyPointsToRedeem = 0)
    {
        // 1. Calculate subtotal from all items
        order.CalculateSubtotal();

        // 2. Calculate member discount
        order.CalculateMemberDiscount();

        // 3. Apply loyalty points redemption
        var loyaltyPointsDiscount = CalculateLoyaltyPointsDiscount(loyaltyPointsToRedeem);
        order.DiscountAmount += loyaltyPointsDiscount;

        // 4. Calculate tax
        order.CalculateTax();

        // 5. Calculate final total
        order.CalculateTotal();

        // 6. Prevent negative totals
        if (order.TotalAmount < 0)
        {
            // Reduce the loyalty points discount to prevent negative total
            var excessDiscount = Math.Abs(order.TotalAmount);
            order.DiscountAmount -= excessDiscount;
            order.CalculateTotal();
        }
    }

    /// <summary>
    /// Calculates discount amount from loyalty points redemption
    /// </summary>
    /// <param name="loyaltyPoints">Number of loyalty points to redeem</param>
    /// <returns>Discount amount (100 points = $1)</returns>
    public decimal CalculateLoyaltyPointsDiscount(int loyaltyPoints)
    {
        if (loyaltyPoints < 0)
        {
            throw new ArgumentException("Loyalty points cannot be negative", nameof(loyaltyPoints));
        }

        return loyaltyPoints * LoyaltyPointsToDiscountRate;
    }

    /// <summary>
    /// Calculates loyalty points earned from the order total
    /// </summary>
    /// <param name="totalAmount">Order total amount</param>
    /// <returns>Loyalty points earned (1 point per $1 spent)</returns>
    public int CalculateLoyaltyPointsEarned(decimal totalAmount)
    {
        if (totalAmount < 0)
        {
            return 0;
        }

        // 1 point per $1 spent (rounded down)
        return (int)Math.Floor(totalAmount);
    }

    /// <summary>
    /// Validates that customer has enough loyalty points for redemption
    /// </summary>
    /// <param name="customer">The customer</param>
    /// <param name="pointsToRedeem">Number of points to redeem</param>
    /// <returns>True if customer has enough points, false otherwise</returns>
    public bool ValidateLoyaltyPointsRedemption(Customer customer, int pointsToRedeem)
    {
        if (pointsToRedeem < 0)
        {
            return false;
        }

        return customer.LoyaltyPoints >= pointsToRedeem;
    }

    /// <summary>
    /// Gets the member discount percentage for a membership tier
    /// </summary>
    /// <param name="tier">Membership tier</param>
    /// <returns>Discount percentage as a decimal (e.g., 0.15 for 15%)</returns>
    public decimal GetMemberDiscountPercentage(MembershipTier tier)
    {
        return tier switch
        {
            MembershipTier.Bronze => 0.05m,
            MembershipTier.Silver => 0.10m,
            MembershipTier.Gold => 0.15m,
            _ => 0m
        };
    }
}
