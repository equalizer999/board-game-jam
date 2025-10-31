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
