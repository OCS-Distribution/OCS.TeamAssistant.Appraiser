using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;

public interface IStartStorySelectionCommand : IRequest<StartStorySelectionResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
}