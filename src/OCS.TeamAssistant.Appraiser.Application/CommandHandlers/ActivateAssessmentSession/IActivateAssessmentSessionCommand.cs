using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessmentSession;

public interface IActivateAssessmentSessionCommand : IRequest<ActivateAssessmentSessionResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
	string Title { get; }
}