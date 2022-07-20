using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;

internal sealed class ResetEstimateCommandHandler : IRequestHandler<ResetEstimateCommand, ResetEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ResetEstimateCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<ResetEstimateResult> Handle(ResetEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);

        var targetState = AssessmentSessionState.Active;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForAppraiser, targetState, command.ModeratorName);

        assessmentSession
            .AsModerator(moderatorId)
            .CurrentStory.Reset();

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        var appraiserIds = assessmentSession.Appraisers.Select(a => a.Id.Value).ToArray();
        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(s => new EstimateItem(s.Appraiser.Id.Value, s.Appraiser.Name, s.StoryExternalId, s.Value))
            .ToArray();
        
        return new ResetEstimateResult(assessmentSession.CurrentStory.Title, appraiserIds, items);
    }
}