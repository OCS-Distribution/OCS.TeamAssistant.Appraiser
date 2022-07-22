using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;

internal sealed class StartStorySelectionCommandHandler
    : IRequestHandler<IStartStorySelectionCommand, StartStorySelectionResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public StartStorySelectionCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<StartStorySelectionResult> Handle(
        IStartStorySelectionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(AssessmentSessionState.Active, command.ModeratorName);

		assessmentSession
            .AsModerator(moderatorId)
            .StartStorySelection();

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new();
    }
}