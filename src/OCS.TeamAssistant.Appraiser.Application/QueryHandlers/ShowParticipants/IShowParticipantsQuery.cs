using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;

public interface IShowParticipantsQuery : IRequest<ShowParticipantsResult>
{
	long ModeratorId { get; }
	string ModeratorName { get; }
}