using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessmentSession;

internal sealed class ActivateAssessmentSessionCommandHandler
    : IRequestHandler<ActivateAssessmentSessionCommand, ActivateAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ActivateAssessmentSessionCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<ActivateAssessmentSessionResult> Handle(
        ActivateAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);

        var targetState = AssessmentSessionState.Draft;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForModerator, targetState, command.ModeratorName);

        assessmentSession
            .AsModerator(moderatorId)
            .Activate(command.Title);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);
        
        return new ActivateAssessmentSessionResult(assessmentSession.Id.Value, assessmentSession.Title);
    }
}