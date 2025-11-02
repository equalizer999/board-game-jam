using BoardGameCafe.Domain;
using BoardGameCafe.Tests.Unit.Builders;
using BoardGameCafe.Tests.Unit.TestUtilities;
using FluentAssertions;

namespace BoardGameCafe.Tests.Unit.Validators;

public class ReservationValidatorTests
{
    private readonly ReservationValidator _validator = new();

    #region Party Size Validation Tests

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(10, true)]
    [InlineData(20, true)]
    public void ValidatePartySize_ValidSizes_ReturnsTrue(int partySize, bool expected)
    {
        // Act
        var result = _validator.ValidatePartySize(partySize);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(21, false)]
    [InlineData(100, false)]
    public void ValidatePartySize_InvalidSizes_ReturnsFalse(int partySize, bool expected)
    {
        // Act
        var result = _validator.ValidatePartySize(partySize);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ValidatePartySize_MinimumValid_ReturnsTrue()
    {
        // Arrange & Act
        var result = _validator.ValidatePartySize(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidatePartySize_MaximumValid_ReturnsTrue()
    {
        // Arrange & Act
        var result = _validator.ValidatePartySize(20);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Future Date Validation Tests

    [Fact]
    public void ValidateFutureDate_Today_ReturnsTrue()
    {
        // Arrange
        var reservationDate = DateTime.Today;
        var currentDate = DateTime.Today;

        // Act
        var result = _validator.ValidateFutureDate(reservationDate, currentDate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateFutureDate_Tomorrow_ReturnsTrue()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(1);
        var currentDate = DateTime.Today;

        // Act
        var result = _validator.ValidateFutureDate(reservationDate, currentDate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateFutureDate_FarFuture_ReturnsTrue()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(30);
        var currentDate = DateTime.Today;

        // Act
        var result = _validator.ValidateFutureDate(reservationDate, currentDate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateFutureDate_Yesterday_ReturnsFalse()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(-1);
        var currentDate = DateTime.Today;

        // Act
        var result = _validator.ValidateFutureDate(reservationDate, currentDate);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateFutureDate_LastWeek_ReturnsFalse()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(-7);
        var currentDate = DateTime.Today;

        // Act
        var result = _validator.ValidateFutureDate(reservationDate, currentDate);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateFutureDate_NoCurrentDateProvided_UsesToday()
    {
        // Arrange
        var reservationDate = DateTime.Today.AddDays(1);

        // Act
        var result = _validator.ValidateFutureDate(reservationDate);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(-7, false)]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(7, true)]
    [InlineData(30, true)]
    public void ValidateFutureDate_VariousDaysOffset_ReturnsCorrectResult(int daysOffset, bool expected)
    {
        // Arrange
        var currentDate = DateTime.Today;
        var reservationDate = currentDate.AddDays(daysOffset);

        // Act
        var result = _validator.ValidateFutureDate(reservationDate, currentDate);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Time Range Validation Tests

    [Fact]
    public void ValidateTimeRange_WithinBusinessHours_ReturnsTrue()
    {
        // Arrange
        var startTime = new TimeSpan(14, 0, 0); // 2 PM
        var endTime = new TimeSpan(16, 0, 0);   // 4 PM

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateTimeRange_AtOpeningTime_ReturnsTrue()
    {
        // Arrange
        var startTime = new TimeSpan(10, 0, 0); // 10 AM (opening)
        var endTime = new TimeSpan(12, 0, 0);   // 12 PM

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateTimeRange_AtClosingTime_ReturnsTrue()
    {
        // Arrange
        var startTime = new TimeSpan(20, 0, 0); // 8 PM
        var endTime = new TimeSpan(22, 0, 0);   // 10 PM (closing)

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateTimeRange_BeforeBusinessHours_ReturnsFalse()
    {
        // Arrange
        var startTime = new TimeSpan(8, 0, 0);  // 8 AM (before opening)
        var endTime = new TimeSpan(10, 0, 0);   // 10 AM

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimeRange_AfterBusinessHours_ReturnsFalse()
    {
        // Arrange
        var startTime = new TimeSpan(20, 0, 0); // 8 PM
        var endTime = new TimeSpan(23, 0, 0);   // 11 PM (after closing)

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimeRange_StartEqualsEnd_ReturnsFalse()
    {
        // Arrange
        var startTime = new TimeSpan(14, 0, 0);
        var endTime = new TimeSpan(14, 0, 0);

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimeRange_EndBeforeStart_ReturnsFalse()
    {
        // Arrange
        var startTime = new TimeSpan(16, 0, 0);
        var endTime = new TimeSpan(14, 0, 0);

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTimeRange_FullDay_WithinHours_ReturnsTrue()
    {
        // Arrange
        var startTime = new TimeSpan(10, 0, 0); // Opening
        var endTime = new TimeSpan(22, 0, 0);   // Closing

        // Act
        var result = _validator.ValidateTimeRange(startTime, endTime);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Table Capacity Validation Tests

    [Fact]
    public void ValidateTableCapacity_SufficientCapacity_ReturnsTrue()
    {
        // Arrange
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = 6
        };
        var partySize = 4;

        // Act
        var result = _validator.ValidateTableCapacity(table, partySize);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateTableCapacity_ExactCapacity_ReturnsTrue()
    {
        // Arrange
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = 4
        };
        var partySize = 4;

        // Act
        var result = _validator.ValidateTableCapacity(table, partySize);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateTableCapacity_InsufficientCapacity_ReturnsFalse()
    {
        // Arrange
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = 4
        };
        var partySize = 6;

        // Act
        var result = _validator.ValidateTableCapacity(table, partySize);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateTableCapacity_NullTable_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => _validator.ValidateTableCapacity(null!, 4));
    }

    [Theory]
    [InlineData(6, 4, true)]
    [InlineData(6, 6, true)]
    [InlineData(6, 7, false)]
    [InlineData(2, 1, true)]
    [InlineData(2, 2, true)]
    [InlineData(2, 3, false)]
    public void ValidateTableCapacity_VariousSizes_ReturnsCorrectResult(int tableCapacity, int partySize, bool expected)
    {
        // Arrange
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = tableCapacity
        };

        // Act
        var result = _validator.ValidateTableCapacity(table, partySize);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Full Reservation Validation Tests

    [Fact]
    public void ValidateReservation_ValidReservation_ReturnsTrue()
    {
        // Arrange
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = 6
        };

        var reservation = new ReservationBuilder()
            .ForTomorrow()
            .WithPartySize(4)
            .WithTime(new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0))
            .Build();

        // Act
        var (isValid, errorMessage) = _validator.ValidateReservation(reservation, table);

        // Assert
        isValid.Should().BeTrue();
        errorMessage.Should().BeNull();
    }

    [Fact]
    public void ValidateReservation_InvalidPartySize_ReturnsFalseWithMessage()
    {
        // Arrange
        var reservation = new ReservationBuilder()
            .ForTomorrow()
            .WithPartySize(25) // Invalid
            .WithTime(new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0))
            .Build();

        // Act
        var (isValid, errorMessage) = _validator.ValidateReservation(reservation);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Contain("Party size");
    }

    [Fact]
    public void ValidateReservation_PastDate_ReturnsFalseWithMessage()
    {
        // Arrange
        var reservation = new ReservationBuilder()
            .InPast()
            .WithPartySize(4)
            .WithTime(new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0))
            .Build();

        // Act
        var (isValid, errorMessage) = _validator.ValidateReservation(reservation);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Contain("future");
    }

    [Fact]
    public void ValidateReservation_OutsideBusinessHours_ReturnsFalseWithMessage()
    {
        // Arrange
        var reservation = new ReservationBuilder()
            .ForTomorrow()
            .WithPartySize(4)
            .OutsideBusinessHours()
            .Build();

        // Act
        var (isValid, errorMessage) = _validator.ValidateReservation(reservation);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Contain("business hours");
    }

    [Fact]
    public void ValidateReservation_InsufficientTableCapacity_ReturnsFalseWithMessage()
    {
        // Arrange
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = 4
        };

        var reservation = new ReservationBuilder()
            .ForTomorrow()
            .WithPartySize(6) // More than table capacity
            .WithTime(new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0))
            .Build();

        // Act
        var (isValid, errorMessage) = _validator.ValidateReservation(reservation, table);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Contain("capacity");
    }

    [Fact]
    public void ValidateReservation_NoTable_ValidatesOtherRules()
    {
        // Arrange
        var reservation = new ReservationBuilder()
            .ForTomorrow()
            .WithPartySize(4)
            .WithTime(new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0))
            .Build();

        // Act
        var (isValid, errorMessage) = _validator.ValidateReservation(reservation, table: null);

        // Assert
        isValid.Should().BeTrue();
        errorMessage.Should().BeNull();
    }

    [Fact]
    public void ValidateReservation_NullReservation_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => _validator.ValidateReservation(null!));
    }

    [Fact]
    public void ValidateReservation_WithBuilder_AllValidationsPassing()
    {
        // Arrange
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T5",
            SeatingCapacity = 8
        };

        var reservation = new ReservationBuilder()
            .ForTomorrow()
            .WithPartySize(6)
            .DuringBusinessHours()
            .WithSpecialRequests("Window seat preferred")
            .Build();

        // Act
        var (isValid, errorMessage) = _validator.ValidateReservation(reservation, table);

        // Assert
        isValid.Should().BeTrue();
        errorMessage.Should().BeNull();
    }

    #endregion
}
