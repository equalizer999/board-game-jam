using BoardGameCafe.Domain;

namespace BoardGameCafe.Tests.Unit.Builders;

/// <summary>
/// Test data builder for Game entities using fluent API
/// </summary>
public class GameBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Test Game";
    private string _publisher = "Test Publisher";
    private int _minPlayers = 2;
    private int _maxPlayers = 4;
    private int _playTimeMinutes = 60;
    private int _ageRating = 10;
    private decimal _complexity = 2.5m;
    private GameCategory _category = GameCategory.Strategy;
    private int _copiesOwned = 3;
    private int _copiesInUse = 0;
    private decimal _dailyRentalFee = 5.00m;
    private string? _description = "A test game description";
    private string? _imageUrl = null;

    public GameBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GameBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public GameBuilder WithPublisher(string publisher)
    {
        _publisher = publisher;
        return this;
    }

    public GameBuilder WithPlayers(int min, int max)
    {
        _minPlayers = min;
        _maxPlayers = max;
        return this;
    }

    public GameBuilder WithPlayTime(int minutes)
    {
        _playTimeMinutes = minutes;
        return this;
    }

    public GameBuilder WithAgeRating(int ageRating)
    {
        _ageRating = ageRating;
        return this;
    }

    public GameBuilder WithComplexity(decimal complexity)
    {
        _complexity = complexity;
        return this;
    }

    public GameBuilder WithCategory(GameCategory category)
    {
        _category = category;
        return this;
    }

    public GameBuilder WithCopies(int owned, int inUse = 0)
    {
        _copiesOwned = owned;
        _copiesInUse = inUse;
        return this;
    }

    public GameBuilder WithDailyRentalFee(decimal fee)
    {
        _dailyRentalFee = fee;
        return this;
    }

    public GameBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public GameBuilder WithImageUrl(string? imageUrl)
    {
        _imageUrl = imageUrl;
        return this;
    }

    public GameBuilder AllCopiesInUse()
    {
        _copiesInUse = _copiesOwned;
        return this;
    }

    public GameBuilder Available()
    {
        _copiesInUse = 0;
        return this;
    }

    public Game Build()
    {
        return new Game
        {
            Id = _id,
            Title = _title,
            Publisher = _publisher,
            MinPlayers = _minPlayers,
            MaxPlayers = _maxPlayers,
            PlayTimeMinutes = _playTimeMinutes,
            AgeRating = _ageRating,
            Complexity = _complexity,
            Category = _category,
            CopiesOwned = _copiesOwned,
            CopiesInUse = _copiesInUse,
            DailyRentalFee = _dailyRentalFee,
            Description = _description,
            ImageUrl = _imageUrl
        };
    }

    public static implicit operator Game(GameBuilder builder) => builder.Build();
}
