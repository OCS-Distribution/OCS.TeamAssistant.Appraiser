using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;

public sealed record EndAssessmentSessionCommand(long ModeratorId, string ModeratorName)
    : IRequest<EndAssessmentSessionResult>;