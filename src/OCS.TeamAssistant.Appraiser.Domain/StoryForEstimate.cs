using OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;

namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class StoryForEstimate
{
    public Appraiser Appraiser { get; private set; } = default!;
    public int StoryExternalId { get; private set; }
    public AssessmentValue Value { get; private set; }

    private StoryForEstimate()
    {
    }

    public static StoryForEstimate Create(Appraiser appraiser, int storyExternalId)
    {
        if (appraiser is null)
            throw new ArgumentNullException(nameof(appraiser));
        
        return new()
        {
            Appraiser = appraiser,
            StoryExternalId = storyExternalId,
            Value = AssessmentValue.None
        };
    }

    public StoryForEstimate SetValue(int? value)
    {
        Value = value.HasValue
            ? (AssessmentValue)value
            : AssessmentValue.Unknown;

        return this;
    }

    internal void Reset() => Value = AssessmentValue.None;
}