using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;

public sealed record ActivateAssessmentSessionCommand(long ModeratorId, string Title)
    : IRequest<ActivateAssessmentSessionResult>;