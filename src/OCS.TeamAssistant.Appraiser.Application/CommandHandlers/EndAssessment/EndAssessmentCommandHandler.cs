using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessment;

internal sealed class EndAssessmentCommandHandler : IRequestHandler<IEndAssessmentCommand, EndAssessmentResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;
	private readonly IMessagesService _messagesService;

    public EndAssessmentCommandHandler(
		IAssessmentSessionRepository assessmentSessionRepository,
		IMessagesService messagesService)
	{
		_assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
		_messagesService = messagesService ?? throw new ArgumentNullException(nameof(messagesService));
	}

    public async Task<EndAssessmentResult> Handle(
        IEndAssessmentCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new ParticipantId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(command.ModeratorName);

		await _assessmentSessionRepository.Remove(assessmentSession, cancellationToken);

		await _messagesService.AssessmentSessionsListChanged();

        var appraiserIds = assessmentSession.Participants.Select(a => a.Id.Value).ToArray();
        return new(assessmentSession.Title, appraiserIds);
    }
}