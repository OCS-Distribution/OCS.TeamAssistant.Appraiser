using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;

internal sealed class EstimateStoryCommandHandler : IRequestHandler<EstimateStoryCommand, EstimateStoryResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EstimateStoryCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<EstimateStoryResult> Handle(EstimateStoryCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new AppraiserId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository.Find(appraiserId, cancellationToken);

        var targetState = AssessmentSessionState.Active;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForAppraiser, targetState, command.AppraiserName);

        if (assessmentSession.CurrentStory.EstimateEnded())
            throw new AppraiserException(MessageId.EstimateRejected, assessmentSession.CurrentStory.Title);

        var appraiser = assessmentSession.Appraisers.Single(a => a.Id == appraiserId);
        assessmentSession.CurrentStory.Estimate(appraiser, command.Value);
        
        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(s => new EstimateItem(s.Appraiser.Id.Value, s.Appraiser.Name, s.StoryExternalId, s.Value))
            .ToArray();

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);
        
        return new EstimateStoryResult(
            assessmentSession.CurrentStory.EstimateEnded(),
            assessmentSession.CurrentStory.Title,
            items);
    }
}