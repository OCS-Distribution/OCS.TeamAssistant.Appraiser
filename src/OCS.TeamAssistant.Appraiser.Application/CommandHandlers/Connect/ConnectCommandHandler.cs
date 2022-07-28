using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Connect;

internal sealed class ConnectCommandHandler : IRequestHandler<IConnectCommand, ConnectResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ConnectCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<ConnectResult> Handle(
        IConnectCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new ParticipantId(command.AppraiserId);
        var existSessionAssessmentSession = await _assessmentSessionRepository.Find(appraiserId, cancellationToken);
        if (existSessionAssessmentSession is not null)
        {
            var messageId = existSessionAssessmentSession.Id.Value == command.AssessmentSessionId
                ? MessageId.AppraiserConnectWithError
                : MessageId.AppraiserConnectedToOtherSession;

            throw new AppraiserUserException(messageId, command.AppraiserName, existSessionAssessmentSession.Title);
        }

        var assessmentSession = await _assessmentSessionRepository
			.Find(new AssessmentSessionId(command.AssessmentSessionId), cancellationToken)
			.EnsureForAppraiser(command.AppraiserName);

		assessmentSession.Connect(appraiserId, command.AppraiserName);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new(assessmentSession.ChatId, assessmentSession.Title, command.AppraiserName);
    }
}