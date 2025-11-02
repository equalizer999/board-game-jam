using BoardGameCafe.Domain;

namespace BoardGameCafe.Tests.Unit.Builders;

/// <summary>
/// Test data builder for Order entities using fluent API
/// </summary>
public class OrderBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid? _reservationId = null;
    private Reservation? _reservation = null;
    private Guid _customerId = Guid.NewGuid();
    private Customer? _customer = null;
    private DateTime _orderDate = DateTime.UtcNow;
    private OrderStatus _status = OrderStatus.Draft;
    private decimal _subtotal = 0;
    private decimal _discountAmount = 0;
    private decimal _taxAmount = 0;
    private decimal _totalAmount = 0;
    private PaymentMethod _paymentMethod = PaymentMethod.Card;
    private List<OrderItem> _items = new();

    public OrderBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public OrderBuilder WithReservation(Reservation reservation)
    {
        _reservation = reservation;
        _reservationId = reservation.Id;
        return this;
    }

    public OrderBuilder WithCustomer(Customer customer)
    {
        _customer = customer;
        _customerId = customer.Id;
        return this;
    }

    public OrderBuilder WithOrderDate(DateTime orderDate)
    {
        _orderDate = orderDate;
        return this;
    }

    public OrderBuilder WithStatus(OrderStatus status)
    {
        _status = status;
        return this;
    }

    public OrderBuilder WithSubtotal(decimal subtotal)
    {
        _subtotal = subtotal;
        return this;
    }

    public OrderBuilder WithDiscountAmount(decimal discountAmount)
    {
        _discountAmount = discountAmount;
        return this;
    }

    public OrderBuilder WithTaxAmount(decimal taxAmount)
    {
        _taxAmount = taxAmount;
        return this;
    }

    public OrderBuilder WithTotalAmount(decimal totalAmount)
    {
        _totalAmount = totalAmount;
        return this;
    }

    public OrderBuilder WithPaymentMethod(PaymentMethod paymentMethod)
    {
        _paymentMethod = paymentMethod;
        return this;
    }

    public OrderBuilder WithItem(MenuItem menuItem, int quantity = 1, decimal? unitPrice = null)
    {
        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = _id,
            MenuItemId = menuItem.Id,
            MenuItem = menuItem,
            Quantity = quantity,
            UnitPrice = unitPrice ?? menuItem.Price
        };
        _items.Add(orderItem);
        return this;
    }

    public OrderBuilder WithItems(params OrderItem[] items)
    {
        _items.AddRange(items);
        return this;
    }

    public OrderBuilder ClearItems()
    {
        _items.Clear();
        return this;
    }

    public Order Build()
    {
        var order = new Order
        {
            Id = _id,
            ReservationId = _reservationId,
            Reservation = _reservation,
            CustomerId = _customerId,
            Customer = _customer ?? new CustomerBuilder().WithId(_customerId).Build(),
            OrderDate = _orderDate,
            Status = _status,
            Subtotal = _subtotal,
            DiscountAmount = _discountAmount,
            TaxAmount = _taxAmount,
            TotalAmount = _totalAmount,
            PaymentMethod = _paymentMethod,
            Items = _items
        };

        // Update OrderId references
        foreach (var item in order.Items)
        {
            item.OrderId = order.Id;
            item.Order = order;
        }

        return order;
    }

    public static implicit operator Order(OrderBuilder builder) => builder.Build();
}
