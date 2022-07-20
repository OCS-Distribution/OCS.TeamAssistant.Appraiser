using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectAppraiser;

internal sealed class ConnectAppraiserCommandHandler : IRequestHandler<ConnectAppraiserCommand, ConnectAppraiserResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ConnectAppraiserCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<ConnectAppraiserResult> Handle(
        ConnectAppraiserCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new AppraiserId(command.AppraiserId);
        var existSessionAssessmentSession = await _assessmentSessionRepository.Find(appraiserId, cancellationToken);
        if (existSessionAssessmentSession is not null)
        {
            var messageId = existSessionAssessmentSession.Id.Value == command.AssessmentSessionId
                ? MessageId.AppraiserConnectWithError
                : MessageId.AppraiserConnectedToOtherSession;
            
            throw new AppraiserException(messageId, command.AppraiserName, existSessionAssessmentSession.Title);
        }

        var assessmentSessionId = new AssessmentSessionId(command.AssessmentSessionId);
        var assessmentSession = await _assessmentSessionRepository.Find(assessmentSessionId, cancellationToken);

        var targetState = AssessmentSessionState.Active;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForAppraiser, targetState, command.AppraiserName);
        
        assessmentSession.ConnectAppraiser(appraiserId, command.AppraiserName);
        
        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);
        
        return new ConnectAppraiserResult(assessmentSession.ChatId, assessmentSession.Title, command.AppraiserName);
    }
}