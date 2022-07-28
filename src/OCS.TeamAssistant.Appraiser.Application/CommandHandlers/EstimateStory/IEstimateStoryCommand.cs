using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;

public interface IEstimateStoryCommand : IRequest<EstimateStoryResult>
{
	long AppraiserId { get; }
	string AppraiserName { get; }
	int? Value { get; }
}