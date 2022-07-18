using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ResetEstimate;

public sealed record ResetEstimateCommand(long ModeratorId, string ModeratorName) : IRequest<ResetEstimateResult>;