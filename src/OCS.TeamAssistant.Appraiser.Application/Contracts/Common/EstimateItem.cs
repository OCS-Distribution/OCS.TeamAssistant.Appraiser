namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Common;

public sealed record EstimateItem(
    long AppraiserId,
    string AppraiserName,
    int StoryExternalId,
    int? Value,
    string DisplayValue);