using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.Restart;
using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Restart;

internal sealed class RestartCommandHandler : IRequestHandler<RestartCommand, RestartResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public RestartCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<RestartResult> Handle(RestartCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        await _assessmentSessionRepository.RemoveAll(cancellationToken);
        
        return new RestartResult();
    }
}