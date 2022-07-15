using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.DisconnectAppraiser;

public sealed record DisconnectAppraiserCommand(long AppraiserId, string AppraiserName)
    : IRequest<DisconnectAppraiserResult>;