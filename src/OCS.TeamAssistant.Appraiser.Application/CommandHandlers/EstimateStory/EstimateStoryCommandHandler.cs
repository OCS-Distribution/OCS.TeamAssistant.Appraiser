using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;

internal sealed class EstimateStoryCommandHandler : IRequestHandler<IEstimateStoryCommand, EstimateStoryResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EstimateStoryCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<EstimateStoryResult> Handle(IEstimateStoryCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new AppraiserId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(appraiserId, cancellationToken)
			.EnsureForAppraiser(AssessmentSessionState.Active, command.AppraiserName);

		if (assessmentSession.CurrentStory.EstimateEnded())
			throw new AppraiserUserException(MessageId.EstimateRejected, assessmentSession.CurrentStory.Title);

		var appraiser = assessmentSession.Appraisers.Single(a => a.Id == appraiserId);
        assessmentSession.CurrentStory.Estimate(appraiser, command.Value);

		var estimateEnded = assessmentSession.CurrentStory.EstimateEnded();
		var title = assessmentSession.CurrentStory.Title;
		var items = assessmentSession.CurrentStory.StoryForEstimates
			.Select(s => new EstimateStoryItem(s.Appraiser.Id.Value, s.Appraiser.Name, s.StoryExternalId, s.Value))
			.ToArray();

		if (estimateEnded)
			assessmentSession.MoveToComplete();

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new(estimateEnded, title, items);
    }
}