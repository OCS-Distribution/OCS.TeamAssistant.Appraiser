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
            throw new AppraiserException("Не удалось обнаружить активную сессию.");
        
        var itemsLookup = assessmentSession.CurrentStory.Assessments
            .GroupBy(a => new { a.Appraiser.Id })
            .Select(a => a.OrderBy(i => i.Created).Last())
            .ToDictionary(a => a.Appraiser.Id);

        var items = assessmentSession.Appraisers
            .Select(a => new EstimateItem(
                a.Id.Value,
                a.Name,
                itemsLookup.TryGetValue(a.Id, out var item) ? item.Value?.ToString() ?? "?" : "-"))
            .ToArray();
        
        return new EndEstimateResult(items);
    }
}