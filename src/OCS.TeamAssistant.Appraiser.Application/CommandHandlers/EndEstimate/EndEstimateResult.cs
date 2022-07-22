namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;

public sealed record EndEstimateResult(string StoryTitle, IReadOnlyCollection<EndEstimateItem> Items);