namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;

public sealed record EstimateStoryResult(
    bool EstimateEnded,
    string StoryTitle,
	decimal? Total,
    IReadOnlyCollection<EstimateStoryItem> Items);