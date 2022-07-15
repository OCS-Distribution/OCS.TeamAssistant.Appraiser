namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public sealed record EstimateItem(
    long AppraiserId,
    string AppraiserName,
    int StoryExternalId,
    int? Value,
    string DisplayValue);