using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;

internal sealed class ResetEstimateCommandHandler : IRequestHandler<ResetEstimateCommand, ResetEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;
    private readonly IReportBuilder _reportBuilder;

    public ResetEstimateCommandHandler(
        IAssessmentSessionRepository assessmentSessionRepository,
        IReportBuilder reportBuilder)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
        _reportBuilder = reportBuilder ?? throw new ArgumentNullException(nameof(reportBuilder));
    }
    
    public async Task<ResetEstimateResult> Handle(ResetEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);
        
        if (assessmentSession?.State != AssessmentSessionState.Active)
            throw new AppraiserException(
                $"Не удалось найти активную сессию для участника {command.ModeratorName}. Обратитесь к модератору.");

        assessmentSession
            .AsModerator(moderatorId)
            .CurrentStory.Reset();

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        var appraiserIds = assessmentSession.Appraisers.Select(a => a.Id.Value).ToArray();
        var items = _reportBuilder.Build(assessmentSession.CurrentStory);
        return new ResetEstimateResult(assessmentSession.CurrentStory.Title, appraiserIds, items);
    }
}