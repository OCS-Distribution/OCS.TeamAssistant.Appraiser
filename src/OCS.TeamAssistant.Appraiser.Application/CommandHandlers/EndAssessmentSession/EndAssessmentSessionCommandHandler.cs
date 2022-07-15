using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;

internal sealed class EndAssessmentSessionCommandHandler
    : IRequestHandler<EndAssessmentSessionCommand, EndAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EndAssessmentSessionCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<EndAssessmentSessionResult> Handle(
        EndAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository.FindByModerator(moderatorId, cancellationToken);
        
        if (assessmentSession?.State != AssessmentSessionState.Active)
            throw new AppraiserException($"Не найдена активная сессия для модератора {command.ModeratorName}.");
        if (!assessmentSession.Moderator.Id.Equals(moderatorId))
            throw new AppraiserException($"У модератора {command.ModeratorName} недостаточно прав для запуска сессии {assessmentSession.Title}.");

        await _assessmentSessionRepository.Remove(assessmentSession, cancellationToken);

        return new EndAssessmentSessionResult(assessmentSession.Title);
    }
}