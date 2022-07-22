using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record AddStoryCommand(long ModeratorId, string ModeratorName, string Title) : IAddStoryCommand;