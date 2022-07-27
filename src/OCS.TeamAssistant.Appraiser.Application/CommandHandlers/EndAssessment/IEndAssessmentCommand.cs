using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessment;

public interface IEndAssessmentCommand : IRequest<EndAssessmentResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
}