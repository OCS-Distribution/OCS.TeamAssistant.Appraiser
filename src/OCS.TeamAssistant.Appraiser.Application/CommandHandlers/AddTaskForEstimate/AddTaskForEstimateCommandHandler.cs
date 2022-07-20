using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTaskForEstimate;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddTaskForEstimate;

internal sealed class AddTaskForEstimateCommandHandler
    : IRequestHandler<AddTaskForEstimateCommand, AddTaskForEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public AddTaskForEstimateCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<AddTaskForEstimateResult> Handle(
        AddTaskForEstimateCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var appraiserId = new AppraiserId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository.Find(
            new AssessmentSessionId(command.AssessmentSessionId),
            cancellationToken);
        var targetState = AssessmentSessionState.Active;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForAppraiser, targetState, command.AppraiserName);
        
        var appraiser = assessmentSession.CurrentStory.Appraisers.Single(a => a.Id == appraiserId);
        assessmentSession.CurrentStory.AddStoryForEstimate(StoryForEstimate.Create(appraiser, command.StoryExternalId));
        
        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new AddTaskForEstimateResult();
    }
}