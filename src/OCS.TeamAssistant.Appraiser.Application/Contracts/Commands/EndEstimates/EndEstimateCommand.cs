using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;

public sealed record EndEstimateCommand(long ModeratorId, string ModeratorName) : IRequest<EndEstimateResult>;