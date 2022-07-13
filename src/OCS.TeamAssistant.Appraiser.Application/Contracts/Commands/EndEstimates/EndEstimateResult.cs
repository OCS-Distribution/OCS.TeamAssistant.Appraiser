namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;

public sealed record EndEstimateResult(IReadOnlyCollection<EstimateItem> Items);