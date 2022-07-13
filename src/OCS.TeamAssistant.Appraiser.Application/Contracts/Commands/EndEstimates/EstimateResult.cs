namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;

public sealed record EstimateResult(
    long AppraiserId,
    string AppraiserName,
    int StoryExternalId,
    int? Value,
    string DisplayValue);