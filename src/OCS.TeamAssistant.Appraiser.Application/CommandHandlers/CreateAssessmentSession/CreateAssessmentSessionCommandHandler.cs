using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;

internal sealed class CreateAssessmentSessionCommandHandler
    : IRequestHandler<CreateAssessmentSessionCommand, CreateAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public CreateAssessmentSessionCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<CreateAssessmentSessionResult> Handle(
        CreateAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var existsSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);

        if (existsSession is null)
        {
            var assessmentSession = AssessmentSession.Create(command.ChatId);
            assessmentSession.ConnectModerator(moderatorId, command.ModeratorName);

            await _assessmentSessionRepository.Add(assessmentSession, cancellationToken);
        }
        else if (existsSession.State == AssessmentSessionState.Active)
        {
            throw new AppraiserException(
                $"Найдена активная сессия \"{existsSession.Title}\" для модератора {existsSession.Moderator.Name}. Необходимо завершить предыдущую сессию перед созданием новой.");
        }

        return new CreateAssessmentSessionResult();
    }
}