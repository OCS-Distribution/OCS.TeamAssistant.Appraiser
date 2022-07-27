using OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record ShowParticipantsQuery(long ModeratorId, string ModeratorName) : IShowParticipantsQuery;