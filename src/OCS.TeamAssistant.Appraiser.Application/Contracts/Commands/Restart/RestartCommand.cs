using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.Restart;

public sealed record RestartCommand() : IRequest<RestartResult>;