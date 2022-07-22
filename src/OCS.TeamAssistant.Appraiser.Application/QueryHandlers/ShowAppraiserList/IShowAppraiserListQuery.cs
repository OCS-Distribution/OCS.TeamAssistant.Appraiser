using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowAppraiserList;

public interface IShowAppraiserListQuery : IRequest<ShowAppraiserListResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
}