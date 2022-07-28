using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;

public interface IEndEstimateCommand : IRequest<EndEstimateResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
}