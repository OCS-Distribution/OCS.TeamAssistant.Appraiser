using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;

internal sealed class AddStoryForEstimateCommandHandler
    : IRequestHandler<IAddStoryForEstimateCommand, AddStoryForEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public AddStoryForEstimateCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<AddStoryForEstimateResult> Handle(
        IAddStoryForEstimateCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new ParticipantId(command.AppraiserId);
		var assessmentSession = await _assessmentSessionRepository
			.Find(new AssessmentSessionId(command.AssessmentSessionId), cancellationToken)
			.EnsureForAppraiser(command.AppraiserName);

		var appraiser = assessmentSession.CurrentStory.Participants.Single(a => a.Id == appraiserId);
        assessmentSession.AddStoryForEstimate(new(appraiser, command.StoryExternalId));

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        return new();
    }
}