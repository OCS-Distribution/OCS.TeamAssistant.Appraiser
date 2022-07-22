namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;

public sealed record ResetEstimateResult(
	string StoryTitle,
	IReadOnlyCollection<long> AppraiserIds,
	IReadOnlyCollection<ResetEstimateItem> Items);