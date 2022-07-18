namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ResetEstimate;

public sealed record ResetEstimateResult(string StoryTitle, IReadOnlyCollection<long> AppraiserIds, IReadOnlyCollection<EstimateItem> Items);