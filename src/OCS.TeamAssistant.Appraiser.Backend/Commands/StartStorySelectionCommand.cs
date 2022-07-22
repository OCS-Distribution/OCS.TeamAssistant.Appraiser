using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record StartStorySelectionCommand(long ModeratorId, string ModeratorName) : IStartStorySelectionCommand;