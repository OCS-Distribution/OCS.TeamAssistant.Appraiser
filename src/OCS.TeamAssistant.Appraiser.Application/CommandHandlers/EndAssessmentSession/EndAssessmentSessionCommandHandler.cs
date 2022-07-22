using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;

internal sealed class EndAssessmentSessionCommandHandler
    : IRequestHandler<IEndAssessmentSessionCommand, EndAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EndAssessmentSessionCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<EndAssessmentSessionResult> Handle(
        IEndAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(AssessmentSessionState.Active, command.ModeratorName);

		assessmentSession
            .AsModerator(moderatorId)
            .MoveToComplete();

        await _assessmentSessionRepository.Remove(assessmentSession, cancellationToken);

        var appraiserIds = assessmentSession.Appraisers.Select(a => a.Id.Value).ToArray();
        return new(assessmentSession.Title, appraiserIds);
    }
}