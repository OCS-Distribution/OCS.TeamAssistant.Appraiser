namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimate;

public sealed record EndEstimateResult(string StoryTitle, IReadOnlyCollection<EstimateItem> Items);