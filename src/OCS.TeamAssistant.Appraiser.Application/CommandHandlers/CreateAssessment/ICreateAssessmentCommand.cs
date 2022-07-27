using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessment;

public interface ICreateAssessmentCommand : IRequest<CreateAssessmentResult>
{
	long ChatId { get; }
	long ModeratorId { get; }
	string ModeratorName { get; }
}