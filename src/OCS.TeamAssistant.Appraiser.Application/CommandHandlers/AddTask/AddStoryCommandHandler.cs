using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddTask;

internal sealed class AddStoryCommandHandler : IRequestHandler<AddStoryCommand, AddStoryResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public AddStoryCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }
    
    public async Task<AddStoryResult> Handle(AddStoryCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new AppraiserId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository.Find(moderatorId, cancellationToken);
        
        if (assessmentSession?.State != AssessmentSessionState.Active)
            throw new AppraiserException($"Не удалось обнаружить активную сессию для модератора {command.ModeratorName}.");

        assessmentSession
            .AsModerator(moderatorId)
            .MoveToNext(command.Title);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        var appraiserIds = assessmentSession.Appraisers.Select(a => a.Id.Value).ToArray();
        return new AddStoryResult(assessmentSession.Id.Value, assessmentSession.CurrentStory.Title, appraiserIds);
    }
}