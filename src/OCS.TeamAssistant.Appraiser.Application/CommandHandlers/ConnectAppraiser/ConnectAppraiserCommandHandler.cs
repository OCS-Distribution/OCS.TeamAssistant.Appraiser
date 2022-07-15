using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
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

        var assessmentSessionId = new AssessmentSessionId(command.AssessmentSessionId);
        var assessmentSession = await _assessmentSessionRepository.FindById(assessmentSessionId, cancellationToken);

        if (assessmentSession?.State != AssessmentSessionState.Active)
            throw new AppraiserException($"Сессия {assessmentSessionId.Value} не найдена. Обратитесь к модератору.");
        
        assessmentSession.ConnectAppraiser(new AppraiserId(command.AppraiserId), command.AppraiserName);
        
        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);
        
        return new ConnectAppraiserResult(assessmentSession.ChatId, assessmentSession.Title, command.AppraiserName);
    }
}