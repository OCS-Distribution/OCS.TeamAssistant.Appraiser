using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimate;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;

internal sealed class EndEstimateCommandHandler : IRequestHandler<EndEstimateCommand, EndEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EndEstimateCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<EndEstimateResult> Handle(EndEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);
        var targetState = AssessmentSessionState.Active;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForModerator, targetState, command.ModeratorName);

        var currentStory = assessmentSession.CurrentStory;
        
        assessmentSession.AsModerator(moderatorId);
        currentStory.MoveToComplete();

        var items = currentStory.StoryForEstimates
            .Select(s => new EstimateItem(s.Appraiser.Id.Value, s.Appraiser.Name, s.StoryExternalId, s.Value))
            .ToArray();
        
        assessmentSession.MoveToComplete();

        return new EndEstimateResult(currentStory.Title, items);
    }
}