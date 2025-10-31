namespace BoardGameCafe.Domain;

public class Table
{
    public Guid Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public TableLocation Location { get; set; }
    public bool IsActive { get; set; } = true;
}

public enum TableLocation
{
    Window,
    Corner,
    Center,
    Private
}
