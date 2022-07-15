using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;

public sealed record CreateAssessmentSessionCommand(long ChatId, long ModeratorId, string ModeratorName)
    : IRequest<CreateAssessmentSessionResult>;