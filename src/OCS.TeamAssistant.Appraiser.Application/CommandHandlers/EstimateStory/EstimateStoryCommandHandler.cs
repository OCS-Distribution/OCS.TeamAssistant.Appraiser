using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;

internal sealed class EstimateStoryCommandHandler : IRequestHandler<EstimateStoryCommand, EstimateStoryResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;
    private readonly IReportBuilder _reportBuilder;

    public EstimateStoryCommandHandler(
        IAssessmentSessionRepository assessmentSessionRepository,
        IReportBuilder reportBuilder)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
        _reportBuilder = reportBuilder ?? throw new ArgumentNullException(nameof(reportBuilder));
    }

    public async Task<EstimateStoryResult> Handle(EstimateStoryCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var appraiserId = new AppraiserId(command.AppraiserId);
        var assessmentSession = await _assessmentSessionRepository.GetByAppraiser(appraiserId, cancellationToken);
        
        if (assessmentSession?.State != AssessmentSessionState.Active)
            throw new AppraiserException(
                $"Не удалось найти активную сессию для участника {command.AppraiserName}. Обратитесь к модератору.");

        var appraiser = assessmentSession.Appraisers.Single(a => a.Id == appraiserId);
        assessmentSession.CurrentStory.Estimate(appraiser, command.Value);
        
        var items = _reportBuilder.Build(assessmentSession.CurrentStory);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);
        
        return new EstimateStoryResult(
            assessmentSession.CurrentStory.EstimateEnded(),
            assessmentSession.CurrentStory.Title,
            items);
    }
}