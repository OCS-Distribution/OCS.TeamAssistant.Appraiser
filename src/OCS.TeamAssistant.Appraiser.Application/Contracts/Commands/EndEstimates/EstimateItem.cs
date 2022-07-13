namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;

public sealed record EstimateItem(long AppraiserId, string AppraiserName, string Value);