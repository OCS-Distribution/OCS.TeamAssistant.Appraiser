using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;

internal sealed class ResetEstimateCommandHandler : IRequestHandler<IResetEstimateCommand, ResetEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ResetEstimateCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<ResetEstimateResult> Handle(IResetEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new ParticipantId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(command.ModeratorName);

		assessmentSession.Reset(moderatorId);

		await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        var appraiserIds = assessmentSession.Participants.Select(a => a.Id.Value).ToArray();
        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(s => new ResetEstimateItem(s.Participant.Id.Value, s.Participant.Name, s.StoryExternalId, s.Value))
            .ToArray();

        return new(
			assessmentSession.CurrentStory.Title,
			appraiserIds,
			assessmentSession.CurrentStory.GetTotal(),
			items);
    }
}