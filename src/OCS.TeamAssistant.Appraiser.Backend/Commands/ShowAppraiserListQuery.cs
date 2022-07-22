using OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowAppraiserList;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record ShowAppraiserListQuery(long ModeratorId, string ModeratorName) : IShowAppraiserListQuery;