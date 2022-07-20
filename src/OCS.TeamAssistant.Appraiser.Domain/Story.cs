using OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class Story
{
    public static readonly Story Empty = new()
    {
        Title = nameof(Story),
        IsActive = false
    };
    
    public string Title { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private readonly List<Appraiser> _appraisers;
    public IReadOnlyCollection<Appraiser> Appraisers => _appraisers;

    private readonly List<StoryForEstimate> _storyForEstimates;
    public IReadOnlyCollection<StoryForEstimate> StoryForEstimates => _storyForEstimates;

    private Story()
    {
        _appraisers = new();
        _storyForEstimates = new();
        IsActive = true;
    }
    
    public static Story Create(string title, IEnumerable<Appraiser> appraisers)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
        if (appraisers is null)
            throw new ArgumentNullException(nameof(appraisers));

        var story = new Story
        {
            Title = title
        };

        foreach (var appraiser in appraisers)
            story.AddAppraiser(appraiser);

        return story;
    }

    public Story Estimate(Appraiser appraiser, int? value)
    {
        if (appraiser is null)
            throw new ArgumentNullException(nameof(appraiser));

        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.Appraiser.Id == appraiser.Id);

        if (storyForEstimate is null)
            throw new AppraiserException(MessageId.MissingTaskForEvaluate);
        
        storyForEstimate.SetValue(value);

        return this;
    }

    private Story AddAppraiser(Appraiser appraiser)
    {
        if (appraiser is null)
            throw new ArgumentNullException(nameof(appraiser));
        
        _appraisers.Add(appraiser);

        return this;
    }

    public Story AddStoryForEstimate(StoryForEstimate storyForEstimate)
    {
        if (storyForEstimate is null)
            throw new ArgumentNullException(nameof(storyForEstimate));
        
        _storyForEstimates.Add(storyForEstimate);

        return this;
    }

    public Story MoveToComplete()
    {
        IsActive = false;

        return this;
    }

    public Story Reset()
    {
        foreach (var storyForEstimate in _storyForEstimates)
            storyForEstimate.Reset();

        IsActive = true;

        return this;
    }

    public bool EstimateEnded()
        => !IsActive || _appraisers.Count == _storyForEstimates.Count(s => s.Value != AssessmentValue.None);
}