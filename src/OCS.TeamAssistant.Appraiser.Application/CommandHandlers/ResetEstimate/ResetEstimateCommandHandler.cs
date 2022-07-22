using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
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

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(AssessmentSessionState.Active, command.ModeratorName);

		assessmentSession
            .AsModerator(moderatorId)
            .Reset();

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        var appraiserIds = assessmentSession.Appraisers.Select(a => a.Id.Value).ToArray();
        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(s => new ResetEstimateItem(s.Appraiser.Id.Value, s.Appraiser.Name, s.StoryExternalId, s.Value))
            .ToArray();

        return new(assessmentSession.CurrentStory.Title, appraiserIds, items);
    }
}