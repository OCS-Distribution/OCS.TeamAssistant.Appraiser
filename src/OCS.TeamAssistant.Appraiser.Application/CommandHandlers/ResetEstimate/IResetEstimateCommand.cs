using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;

public interface IResetEstimateCommand : IRequest<ResetEstimateResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
}