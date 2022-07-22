using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;

public interface ICreateAssessmentSessionCommand : IRequest<CreateAssessmentSessionResult>
{
	long ChatId { get; }
	long ModeratorId { get; }
	string ModeratorName { get; }
}