using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;

public interface IEndAssessmentSessionCommand : IRequest<EndAssessmentSessionResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
}