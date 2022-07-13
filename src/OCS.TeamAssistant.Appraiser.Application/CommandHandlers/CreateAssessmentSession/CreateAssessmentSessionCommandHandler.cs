using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;

internal sealed class CreateAssessmentSessionCommandHandler
    : IRequestHandler<CreateAssessmentSessionCommand, CreateAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public CreateAssessmentSessionCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<CreateAssessmentSessionResult> Handle(
        CreateAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = AssessmentSession.Create();
        assessmentSession.ConnectModerator(new AppraiserId(command.ModeratorId), command.ModeratorName);
        
        await _assessmentSessionRepository.Add(assessmentSession, cancellationToken);

        return new CreateAssessmentSessionResult();
    }
}