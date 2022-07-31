using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;

internal sealed class AddStoryForEstimateCommandHandler
    : IRequestHandler<IAddStoryForEstimateCommand, AddStoryForEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;
	private readonly IMessagesService _messagesService;

    public AddStoryForEstimateCommandHandler(
		IAssessmentSessionRepository assessmentSessionRepository,
		IMessagesService messagesService)
	{
		_assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
		_messagesService = messagesService ?? throw new ArgumentNullException(nameof(messagesService));
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

		await _messagesService.StoryChanged(assessmentSession.Id.Value);

        return new();
    }
}