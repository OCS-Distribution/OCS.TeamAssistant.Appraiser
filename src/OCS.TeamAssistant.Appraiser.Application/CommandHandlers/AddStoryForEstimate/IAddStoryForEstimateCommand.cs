using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;

public interface IAddStoryForEstimateCommand : IRequest<AddStoryForEstimateResult>
{
	Guid AssessmentSessionId { get; }
	long AppraiserId { get; }
	string AppraiserName { get; }
	int StoryExternalId { get; }
}