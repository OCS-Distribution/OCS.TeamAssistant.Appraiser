using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;

public sealed record AddStoryCommand(long ModeratorId, string ModeratorName, string Title) : IRequest<AddStoryResult>;