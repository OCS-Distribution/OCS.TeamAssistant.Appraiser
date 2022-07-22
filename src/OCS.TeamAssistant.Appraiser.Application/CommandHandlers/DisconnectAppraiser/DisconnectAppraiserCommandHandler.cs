using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;

internal sealed class DisconnectAppraiserCommandHandler
    : IRequestHandler<IDisconnectAppraiserCommand, DisconnectAppraiserResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public DisconnectAppraiserCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<DisconnectAppraiserResult> Handle(
        IDisconnectAppraiserCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new AppraiserId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(appraiserId, cancellationToken)
			.EnsureForAppraiser(AssessmentSessionState.Active, command.AppraiserName);

		assessmentSession.DisconnectAppraiser(appraiserId);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new(assessmentSession.ChatId, assessmentSession.Title, command.AppraiserName);
    }
}