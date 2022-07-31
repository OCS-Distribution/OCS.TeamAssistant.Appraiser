namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

public sealed record AddStoryResult(
    Guid AssessmentSessionId,
    string Title,
    IReadOnlyCollection<AddStoryItem> Items);