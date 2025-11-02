using BoardGameCafe.Domain;

namespace BoardGameCafe.Tests.Unit.Builders;

/// <summary>
/// Test data builder for Reservation entities using fluent API
/// </summary>
public class ReservationBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _customerId = Guid.NewGuid();
    private Guid _tableId = Guid.NewGuid();
    private DateTime _reservationDate = DateTime.Today.AddDays(1);
    private TimeSpan _startTime = new TimeSpan(18, 0, 0); // 6 PM
    private TimeSpan _endTime = new TimeSpan(20, 0, 0);   // 8 PM
    private int _partySize = 4;
    private ReservationStatus _status = ReservationStatus.Pending;
    private DateTime _createdAt = DateTime.UtcNow;
    private string? _specialRequests = null;
    private Customer? _customer = null;
    private Table? _table = null;

    public ReservationBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ReservationBuilder WithCustomer(Customer customer)
    {
        _customer = customer;
        _customerId = customer.Id;
        return this;
    }

    public ReservationBuilder WithTable(Table table)
    {
        _table = table;
        _tableId = table.Id;
        return this;
    }

    public ReservationBuilder WithReservationDate(DateTime date)
    {
        _reservationDate = date;
        return this;
    }

    public ReservationBuilder WithTime(TimeSpan startTime, TimeSpan endTime)
    {
        _startTime = startTime;
        _endTime = endTime;
        return this;
    }

    public ReservationBuilder WithPartySize(int partySize)
    {
        _partySize = partySize;
        return this;
    }

    public ReservationBuilder WithStatus(ReservationStatus status)
    {
        _status = status;
        return this;
    }

    public ReservationBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ReservationBuilder WithSpecialRequests(string specialRequests)
    {
        _specialRequests = specialRequests;
        return this;
    }

    public ReservationBuilder ForToday()
    {
        _reservationDate = DateTime.Today;
        return this;
    }

    public ReservationBuilder ForTomorrow()
    {
        _reservationDate = DateTime.Today.AddDays(1);
        return this;
    }

    public ReservationBuilder InPast()
    {
        _reservationDate = DateTime.Today.AddDays(-7);
        return this;
    }

    public ReservationBuilder DuringBusinessHours()
    {
        _startTime = new TimeSpan(14, 0, 0); // 2 PM
        _endTime = new TimeSpan(16, 0, 0);   // 4 PM
        return this;
    }

    public ReservationBuilder OutsideBusinessHours()
    {
        _startTime = new TimeSpan(6, 0, 0);  // 6 AM
        _endTime = new TimeSpan(8, 0, 0);    // 8 AM
        return this;
    }

    public Reservation Build()
    {
        return new Reservation
        {
            Id = _id,
            CustomerId = _customerId,
            TableId = _tableId,
            ReservationDate = _reservationDate,
            StartTime = _startTime,
            EndTime = _endTime,
            PartySize = _partySize,
            Status = _status,
            CreatedAt = _createdAt,
            SpecialRequests = _specialRequests,
            Customer = _customer,
            Table = _table
        };
    }

    public static implicit operator Reservation(ReservationBuilder builder) => builder.Build();
}
