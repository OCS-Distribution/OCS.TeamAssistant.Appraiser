using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimate;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;

internal sealed class EndEstimateCommandHandler : IRequestHandler<EndEstimateCommand, EndEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;
    private readonly IReportBuilder _reportBuilder;

    public EndEstimateCommandHandler(
        IAssessmentSessionRepository assessmentSessionRepository,
        IReportBuilder reportBuilder)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
        _reportBuilder = reportBuilder ?? throw new ArgumentNullException(nameof(reportBuilder));
    }
    
    public async Task<EndEstimateResult> Handle(EndEstimateCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);
        if (assessmentSession?.State != AssessmentSessionState.Active)
            throw new AppraiserException(
                $"Не удалось обнаружить активную сессию для модератора {command.ModeratorName}.");

        var currentStory = assessmentSession.CurrentStory;
        
        assessmentSession
            .AsModerator(moderatorId)
            .MoveToComplete();
        
        var items = _reportBuilder.Build(currentStory.MoveToComplete());
        
        return new EndEstimateResult(currentStory.Title, items);
    }
}