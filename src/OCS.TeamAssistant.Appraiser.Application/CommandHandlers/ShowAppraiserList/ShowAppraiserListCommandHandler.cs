using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;

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
        
        var assessmentSession = await _assessmentSessionRepository.FindByModerator(
            new AppraiserId(command.ModeratorId),
            cancellationToken);

        if (assessmentSession is null)
            throw new AppraiserException("Не удалось обнаружить текущую сессию.");

        var appraisers = assessmentSession.Appraisers.Select(a => a.Name).ToArray();
        
        return new ShowAppraiserListResult(appraisers);
    }
}