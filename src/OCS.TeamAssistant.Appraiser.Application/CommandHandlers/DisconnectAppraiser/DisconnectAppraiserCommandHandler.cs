using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.DisconnectAppraiser;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;

internal sealed class DisconnectAppraiserCommandHandler
    : IRequestHandler<DisconnectAppraiserCommand, DisconnectAppraiserResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public DisconnectAppraiserCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<DisconnectAppraiserResult> Handle(
        DisconnectAppraiserCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new AppraiserId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository.Find(appraiserId, cancellationToken);

        var targetState = AssessmentSessionState.Active;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForAppraiser, targetState, command.AppraiserName);

        assessmentSession.DisconnectAppraiser(appraiserId);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new DisconnectAppraiserResult(assessmentSession.ChatId, assessmentSession.Title, command.AppraiserName);
    }
}