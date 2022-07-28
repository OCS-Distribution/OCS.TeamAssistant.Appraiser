using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

public interface IAddStoryCommand : IRequest<AddStoryResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
	string Title { get; }
}