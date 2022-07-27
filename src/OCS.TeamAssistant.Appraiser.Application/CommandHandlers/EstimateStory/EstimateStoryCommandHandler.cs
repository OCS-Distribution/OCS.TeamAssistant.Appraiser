using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
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

        var appraiserId = new ParticipantId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(appraiserId, cancellationToken)
			.EnsureForAppraiser(command.AppraiserName);

		var appraiser = assessmentSession.Participants.Single(a => a.Id == appraiserId);
        assessmentSession.Estimate(appraiser, command.Value.ToAssessmentValue());

		await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

		var items = assessmentSession.CurrentStory.StoryForEstimates
			.Select(s => new EstimateStoryItem(s.Participant.Id.Value, s.Participant.Name, s.StoryExternalId, s.Value))
			.ToArray();

        return new(
			assessmentSession.EstimateEnded(),
			assessmentSession.CurrentStory.Title,
			assessmentSession.CurrentStory.GetTotal(),
			items);
    }
}