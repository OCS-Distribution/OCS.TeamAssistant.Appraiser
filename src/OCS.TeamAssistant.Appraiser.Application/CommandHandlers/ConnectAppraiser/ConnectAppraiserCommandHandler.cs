using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectAppraiser;

internal sealed class ConnectAppraiserCommandHandler : IRequestHandler<IConnectAppraiserCommand, ConnectAppraiserResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ConnectAppraiserCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<ConnectAppraiserResult> Handle(
        IConnectAppraiserCommand command,
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

            throw new AppraiserUserException(messageId, command.AppraiserName, existSessionAssessmentSession.Title);
        }

        var assessmentSession = await _assessmentSessionRepository
			.Find(new AssessmentSessionId(command.AssessmentSessionId), cancellationToken)
			.EnsureForAppraiser(AssessmentSessionState.Active, command.AppraiserName);

		assessmentSession.ConnectAppraiser(appraiserId, command.AppraiserName);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new(assessmentSession.ChatId, assessmentSession.Title, command.AppraiserName);
    }
}