using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment;

internal sealed class ActivateAssessmentCommandHandler
	: IRequestHandler<IActivateAssessmentCommand, ActivateAssessmentResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;
	private readonly IMessagesService _messagesService;

    public ActivateAssessmentCommandHandler(
		IAssessmentSessionRepository assessmentSessionRepository,
		IMessagesService messagesService)
	{
		_assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
		_messagesService = messagesService  ?? throw new ArgumentNullException(nameof(messagesService));
	}

    public async Task<ActivateAssessmentResult> Handle(
        IActivateAssessmentCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new ParticipantId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(command.ModeratorName);

		assessmentSession.Activate(moderatorId, command.Title);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

		await _messagesService.AssessmentSessionsListChanged();

        return new(assessmentSession.Id.Value, assessmentSession.Title);
    }
}