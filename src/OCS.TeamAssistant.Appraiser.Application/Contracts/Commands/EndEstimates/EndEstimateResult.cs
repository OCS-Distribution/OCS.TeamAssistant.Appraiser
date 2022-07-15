namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;

public sealed record EndEstimateResult(string StoryTitle, IReadOnlyCollection<EstimateItem> Items);