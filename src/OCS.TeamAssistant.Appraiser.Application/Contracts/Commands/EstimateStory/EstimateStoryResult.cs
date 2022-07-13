namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;

public sealed record EstimateStoryResult(string StoryTitle, IReadOnlyCollection<EstimateItem> Items);