using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ShowAppraiserList;

internal sealed class ShowAppraiserListCommandHandler
    : IRequestHandler<ShowAppraiserListCommand, ShowAppraiserListResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ShowAppraiserListCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<ShowAppraiserListResult> Handle(
        ShowAppraiserListCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var assessmentSession = await _assessmentSessionRepository.Find(
            new AppraiserId(command.ModeratorId),
            cancellationToken);

        var targetState = AssessmentSessionState.Active;
        if (assessmentSession?.State != targetState)
            throw new AppraiserException(MessageId.SessionNotFoundForModerator, targetState, command.ModeratorName);

        var appraisers = assessmentSession.Appraisers.Select(a => a.Name).ToArray();
        
        return new ShowAppraiserListResult(appraisers);
    }
}