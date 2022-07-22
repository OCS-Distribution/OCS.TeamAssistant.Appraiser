using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessmentSession;

internal sealed class ActivateAssessmentSessionCommandHandler
    : IRequestHandler<IActivateAssessmentSessionCommand, ActivateAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ActivateAssessmentSessionCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<ActivateAssessmentSessionResult> Handle(
        IActivateAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(AssessmentSessionState.Draft, command.ModeratorName);

		assessmentSession
            .AsModerator(moderatorId)
            .Activate(command.Title);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new(assessmentSession.Id.Value, assessmentSession.Title);
    }
}