namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;

public sealed record AddStoryResult(string Title, long[] AppraiserIds);