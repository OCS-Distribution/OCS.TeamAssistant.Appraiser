using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment;

public interface IActivateAssessmentCommand : IRequest<ActivateAssessmentResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
	string Title { get; }
}