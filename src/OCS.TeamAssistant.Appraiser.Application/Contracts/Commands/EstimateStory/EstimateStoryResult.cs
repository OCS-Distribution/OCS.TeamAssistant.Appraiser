namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;

public sealed record EstimateStoryResult(
    bool EstimateEnded,
    string StoryTitle,
    IReadOnlyCollection<EstimateItem> Items);