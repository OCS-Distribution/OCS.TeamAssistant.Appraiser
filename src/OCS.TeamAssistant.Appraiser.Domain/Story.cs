namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class Story
{
    public static readonly Story Empty = new()
    {
        Title = string.Empty
    };
    
    public string Title { get; private set; } = default!;

    private readonly List<Assessment> _assessments;
    public IReadOnlyCollection<Assessment> Assessments => _assessments;

    private Story()
    {
        _assessments = new();
    }
    
    public static Story Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

        return new()
        {
            Title = title
        };
    }

    public Story Estimate(Appraiser appraiser, int? value)
    {
        if (appraiser is null)
            throw new ArgumentNullException(nameof(appraiser));

        _assessments.Add(Assessment.Create(appraiser, value));

        return this;
    }
}