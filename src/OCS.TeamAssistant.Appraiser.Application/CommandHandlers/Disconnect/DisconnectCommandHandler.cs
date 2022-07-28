using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Disconnect;

internal sealed class DisconnectCommandHandler : IRequestHandler<IDisconnectCommand, DisconnectResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public DisconnectCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<DisconnectResult> Handle(
        IDisconnectCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new ParticipantId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(appraiserId, cancellationToken)
			.EnsureForAppraiser(command.AppraiserName);

		assessmentSession.Disconnect(appraiserId);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new(assessmentSession.ChatId, assessmentSession.Title, command.AppraiserName);
    }
}