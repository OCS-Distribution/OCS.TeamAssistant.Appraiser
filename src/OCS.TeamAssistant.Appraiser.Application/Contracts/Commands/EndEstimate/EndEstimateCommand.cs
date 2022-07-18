using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimate;

public sealed record EndEstimateCommand(long ModeratorId, string ModeratorName) : IRequest<EndEstimateResult>;