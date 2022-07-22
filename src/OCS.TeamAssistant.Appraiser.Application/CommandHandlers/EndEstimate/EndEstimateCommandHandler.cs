using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;

internal sealed class EndEstimateCommandHandler : IRequestHandler<IEndEstimateCommand, EndEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EndEstimateCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<EndEstimateResult> Handle(IEndEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(AssessmentSessionState.Active, command.ModeratorName);

		var title = assessmentSession.CurrentStory.Title;
		var items = assessmentSession.CurrentStory.StoryForEstimates
			.Select(s => new EndEstimateItem(s.Appraiser.Id.Value, s.Appraiser.Name, s.StoryExternalId, s.Value))
			.ToArray();

        assessmentSession
			.AsModerator(moderatorId)
			.MoveToComplete();

        return new(title, items);
    }
}