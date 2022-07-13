using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;

public sealed record EstimateStoryCommand(long AppraiserId, int? Value) : IRequest<EstimateStoryResult>;