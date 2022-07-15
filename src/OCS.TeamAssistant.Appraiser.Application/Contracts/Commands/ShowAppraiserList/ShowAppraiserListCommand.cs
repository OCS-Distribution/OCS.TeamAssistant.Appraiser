using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;

public sealed record ShowAppraiserListCommand(long ModeratorId, string ModeratorName)
    : IRequest<ShowAppraiserListResult>;