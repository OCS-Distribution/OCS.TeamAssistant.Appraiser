using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;

public sealed record ConnectAppraiserCommand(Guid AssessmentSessionId, long AppraiserId, string AppraiserName)
    : IRequest<ConnectAppraiserResult>;