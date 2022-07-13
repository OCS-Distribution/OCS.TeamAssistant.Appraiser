using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;

public sealed record ShowAppraiserListCommand(long ModeratorId) : IRequest<ShowAppraiserListResult>;