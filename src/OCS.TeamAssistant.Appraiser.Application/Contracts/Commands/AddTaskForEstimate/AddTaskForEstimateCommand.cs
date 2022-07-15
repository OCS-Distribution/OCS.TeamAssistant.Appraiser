using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTaskForEstimate;

public sealed record AddTaskForEstimateCommand(Guid AssessmentSessionId, long AppraiserId, string AppraiserName, int StoryExternalId)
    : IRequest<AddTaskForEstimateResult>;