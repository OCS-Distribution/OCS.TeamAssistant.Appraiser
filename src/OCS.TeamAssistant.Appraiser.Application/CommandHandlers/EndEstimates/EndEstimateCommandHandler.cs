using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimates;

internal sealed class EndEstimateCommandHandler : IRequestHandler<EndEstimateCommand, EndEstimateResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EndEstimateCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
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
        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(a => new EstimateResult(
                a.Appraiser.Id.Value,
                a.Appraiser.Name,
                a.StoryExternalId,
                a.GetValue(),
                a.GetDisplayValue()))
            .ToArray();
        
        assessmentSession.End();
        
        return new EndEstimateResult(storyTitle, items);
    }
}