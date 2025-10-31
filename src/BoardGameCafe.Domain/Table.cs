namespace BoardGameCafe.Domain;

public class Table
{
    public Guid Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int SeatingCapacity { get; set; }
    public bool IsWindowSeat { get; set; }
    public bool IsAccessible { get; set; }
    public decimal HourlyRate { get; set; }
    public TableStatus Status { get; set; } = TableStatus.Available;
}

public enum TableStatus
{
    Available,
    Reserved,
    Occupied,
    UnderMaintenance
}
