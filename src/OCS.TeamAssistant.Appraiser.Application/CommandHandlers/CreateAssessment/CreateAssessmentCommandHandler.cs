using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessment;

internal sealed class CreateAssessmentCommandHandler
    : IRequestHandler<ICreateAssessmentCommand, CreateAssessmentResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public CreateAssessmentCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<CreateAssessmentResult> Handle(
        ICreateAssessmentCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new ParticipantId(command.ModeratorId);
        var existsSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);

        if (existsSession is null)
		{
			var moderator = new Participant(moderatorId, command.ModeratorName);
            var assessmentSession = new AssessmentSession(command.ChatId, moderator);

			await _assessmentSessionRepository.Add(assessmentSession, cancellationToken);
        }

		return new();
    }
}