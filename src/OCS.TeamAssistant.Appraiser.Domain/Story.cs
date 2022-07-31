using OCS.TeamAssistant.Appraiser.Domain.States;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class Story : IStoryAccessor
{
	public static readonly Story Empty = new(nameof(Story));

	public string Title { get; }

	private readonly List<StoryForEstimate> _storyForEstimates;
    public IReadOnlyCollection<StoryForEstimate> StoryForEstimates => _storyForEstimates;

    public Story(string title)
    {
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

		_storyForEstimates = new();
		Title = title;
	}

	internal bool EstimateEnded() => StoryForEstimates.All(s => s.Value != AssessmentValue.None);

	public decimal? GetTotal()
	{
		var values = _storyForEstimates
			.Where(i => AssessmentValueRules.GetAssessments.Contains(i.Value))
			.Select(i => (int)i.Value)
			.ToArray();

		var result = EstimateEnded() && values.Any()
			? (decimal?)values.Sum() / values.Length
			: null;

		return result;
	}

	void IStoryAccessor.Estimate(Participant participant, AssessmentValue value)
    {
        if (participant is null)
            throw new ArgumentNullException(nameof(participant));

        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.Participant.Id == participant.Id);

        if (storyForEstimate is null)
            throw new AppraiserUserException(MessageId.MissingTaskForEvaluate);

        storyForEstimate.SetValue(value);
	}

	void IStoryAccessor.AddStoryForEstimate(StoryForEstimate storyForEstimate)
    {
        if (storyForEstimate is null)
            throw new ArgumentNullException(nameof(storyForEstimate));

        _storyForEstimates.Add(storyForEstimate);
	}

	void IStoryAccessor.Reset()
    {
        foreach (var storyForEstimate in _storyForEstimates)
            storyForEstimate.Reset();
	}
}