namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;

public sealed record AddStoryResult(
    Guid AssessmentSessionId,
    string Title,
    long[] AppraiserIds,
    IReadOnlyCollection<EstimateItem> Items);