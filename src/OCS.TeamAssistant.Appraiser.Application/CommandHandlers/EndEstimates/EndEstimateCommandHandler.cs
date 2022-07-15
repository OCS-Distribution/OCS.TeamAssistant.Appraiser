using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimates;

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
        
        var assessmentSession = await _assessmentSessionRepository.FindByModerator(
            new AppraiserId(command.ModeratorId),
            cancellationToken);
        if (assessmentSession?.State != AssessmentSessionState.Active)
            throw new AppraiserException(
                $"Не удалось обнаружить активную сессию для модератора {command.ModeratorName}.");

        var storyTitle = assessmentSession.CurrentStory.Title;
        var items = _reportBuilder.Build(assessmentSession.CurrentStory.End());
        assessmentSession.End();
        
        return new EndEstimateResult(storyTitle, items);
    }
}