using BoardGameCafe.Domain;
using BoardGameCafe.Tests.Unit.Builders;
using BoardGameCafe.Tests.Unit.TestUtilities;
using FluentAssertions;

namespace BoardGameCafe.Tests.Unit.Services;

public class GameAvailabilityServiceTests
{
    private readonly GameAvailabilityService _service = new();

    #region GetAvailableCopies Tests

    [Fact]
    public void GetAvailableCopies_AllCopiesAvailable_ReturnsCorrectCount()
    {
        // Arrange
        var game = new GameBuilder()
            .WithCopies(owned: 5, inUse: 0)
            .Build();

        // Act
        var available = _service.GetAvailableCopies(game);

        // Assert
        available.Should().Be(5);
    }

    [Fact]
    public void GetAvailableCopies_SomeCopiesInUse_ReturnsRemainingCount()
    {
        // Arrange
        var game = new GameBuilder()
            .WithCopies(owned: 5, inUse: 2)
            .Build();

        // Act
        var available = _service.GetAvailableCopies(game);

        // Assert
        available.Should().Be(3);
    }

    [Fact]
    public void GetAvailableCopies_AllCopiesInUse_ReturnsZero()
    {
        // Arrange
        var game = new GameBuilder()
            .WithCopies(owned: 3, inUse: 3)
            .Build();

        // Act
        var available = _service.GetAvailableCopies(game);

        // Assert
        available.Should().Be(0);
    }

    [Fact]
    public void GetAvailableCopies_MoreInUseThanOwned_ReturnsZero()
    {
        // Arrange - Data corruption scenario
        var game = new GameBuilder()
            .WithCopies(owned: 3, inUse: 5)
            .Build();

        // Act
        var available = _service.GetAvailableCopies(game);

        // Assert
        available.Should().Be(0); // Should not return negative
    }

    [Fact]
    public void GetAvailableCopies_NullGame_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetAvailableCopies(null!));
    }

    #endregion

    #region CalculateLateFee Tests

    [Fact]
    public void CalculateLateFee_ReturnedOnTime_ReturnsZero()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = new DateTime(2025, 1, 1, 13, 30, 0);

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        fee.Should().Be(0);
    }

    [Fact]
    public void CalculateLateFee_WithinGracePeriod_ReturnsZero()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = new DateTime(2025, 1, 1, 14, 10, 0); // 10 minutes late, within 15 min grace

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        fee.Should().Be(0);
    }

    [Fact]
    public void CalculateLateFee_ExactlyGracePeriod_ReturnsZero()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = new DateTime(2025, 1, 1, 14, 15, 0); // Exactly 15 minutes

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        fee.Should().Be(0);
    }

    [Fact]
    public void CalculateLateFee_1MinuteOverGracePeriod_Charges1Hour()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = new DateTime(2025, 1, 1, 14, 16, 0); // 16 minutes late (1 min over grace)

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        fee.Should().Be(2.00m); // 1 hour at $2/hour (rounds up)
    }

    [Fact]
    public void CalculateLateFee_1HourLate_AfterGracePeriod()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = new DateTime(2025, 1, 1, 15, 0, 0); // 1 hour late

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        // 60 minutes - 15 grace = 45 minutes = 1 hour (rounded up) = $2
        fee.Should().Be(2.00m);
    }

    [Fact]
    public void CalculateLateFee_2HoursLate_Charges2Hours()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = new DateTime(2025, 1, 1, 16, 0, 0); // 2 hours late

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        // 120 minutes - 15 grace = 105 minutes = 2 hours (rounded up) = $4
        fee.Should().Be(4.00m);
    }

    [Fact]
    public void CalculateLateFee_PartialHour_RoundsUp()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = new DateTime(2025, 1, 1, 15, 30, 0); // 1.5 hours late

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        // 90 minutes - 15 grace = 75 minutes = 1.25 hours (rounds up to 2) = $4
        fee.Should().Be(4.00m);
    }

    [Fact]
    public void CalculateLateFee_NotYetReturned_UsesCurrentTime()
    {
        // Arrange
        var dueBackAt = DateTime.UtcNow.AddHours(-2); // Due 2 hours ago

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt: null);

        // Assert
        // Should be at least $2 (2 hours overdue - 15 min grace)
        fee.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(15, 0.00)]    // Exactly grace period
    [InlineData(16, 2.00)]    // 1 minute over grace = 1 hour
    [InlineData(75, 2.00)]    // 60 minutes over grace = 1 hour
    [InlineData(76, 4.00)]    // 61 minutes over grace = 2 hours (rounds up)
    [InlineData(135, 4.00)]   // 120 minutes over grace = 2 hours
    [InlineData(136, 6.00)]   // 121 minutes over grace = 3 hours (rounds up)
    public void CalculateLateFee_VariousMinutesLate_CalculatesCorrectFee(int minutesLate, decimal expectedFee)
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var returnedAt = dueBackAt.AddMinutes(minutesLate);

        // Act
        var fee = _service.CalculateLateFee(dueBackAt, returnedAt);

        // Assert
        fee.Should().Be(expectedFee);
    }

    #endregion

    #region IsOverdue Tests

    [Fact]
    public void IsOverdue_NotYetDue_ReturnsFalse()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var currentTime = new DateTime(2025, 1, 1, 13, 0, 0);

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt, currentTime);

        // Assert
        isOverdue.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_ExactlyOnTime_ReturnsFalse()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var currentTime = new DateTime(2025, 1, 1, 14, 0, 0);

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt, currentTime);

        // Assert
        isOverdue.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WithinGracePeriod_ReturnsFalse()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var currentTime = new DateTime(2025, 1, 1, 14, 10, 0); // 10 minutes late

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt, currentTime);

        // Assert
        isOverdue.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_ExactlyGracePeriod_ReturnsFalse()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var currentTime = new DateTime(2025, 1, 1, 14, 15, 0); // Exactly 15 minutes

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt, currentTime);

        // Assert
        isOverdue.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_1MinuteOverGracePeriod_ReturnsTrue()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var currentTime = new DateTime(2025, 1, 1, 14, 16, 0); // 16 minutes

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt, currentTime);

        // Assert
        isOverdue.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_SeveralHoursLate_ReturnsTrue()
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var currentTime = new DateTime(2025, 1, 1, 18, 0, 0);

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt, currentTime);

        // Assert
        isOverdue.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_NoCurrentTimeProvided_UsesUtcNow()
    {
        // Arrange
        var dueBackAt = DateTime.UtcNow.AddHours(-1); // Due 1 hour ago

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt);

        // Assert
        isOverdue.Should().BeTrue();
    }

    [Theory]
    [InlineData(-60, false)]  // 1 hour before due
    [InlineData(0, false)]    // Exactly on time
    [InlineData(10, false)]   // 10 minutes late (within grace)
    [InlineData(15, false)]   // 15 minutes late (exactly grace period)
    [InlineData(16, true)]    // 16 minutes late (1 minute over grace)
    [InlineData(60, true)]    // 1 hour late
    [InlineData(120, true)]   // 2 hours late
    public void IsOverdue_VariousTimeDifferences_ReturnsCorrectResult(int minutesDifference, bool expectedOverdue)
    {
        // Arrange
        var dueBackAt = new DateTime(2025, 1, 1, 14, 0, 0);
        var currentTime = dueBackAt.AddMinutes(minutesDifference);

        // Act
        var isOverdue = _service.IsOverdue(dueBackAt, currentTime);

        // Assert
        isOverdue.Should().Be(expectedOverdue);
    }

    #endregion
}
