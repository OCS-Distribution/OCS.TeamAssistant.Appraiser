using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;

internal sealed class EstimateStoryCommandHandler : IRequestHandler<EstimateStoryCommand, EstimateStoryResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public EstimateStoryCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
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
        
        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(a => new EstimateItem(
                a.Appraiser.Id.Value,
                a.Appraiser.Name,
                a.StoryExternalId,
                Exists: a.Value != AssessmentValue.None))
            .ToArray();

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);
        
        return new EstimateStoryResult(assessmentSession.CurrentStory.Title, items);
    }
}