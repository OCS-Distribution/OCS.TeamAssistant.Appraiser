using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

internal sealed class AddStoryCommandHandler : IRequestHandler<IAddStoryCommand, AddStoryResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public AddStoryCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<AddStoryResult> Handle(IAddStoryCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var moderatorId = new ParticipantId(command.ModeratorId);
        var assessmentSession = await _assessmentSessionRepository
			.Find(moderatorId, cancellationToken)
			.EnsureForModerator(command.ModeratorName);

		assessmentSession.StorySelected(moderatorId, command.Title);

        await _assessmentSessionRepository.Update(assessmentSession, cancellationToken);

        var appraiserIds = assessmentSession.CurrentStory.Participants.Select(a => a.Id.Value).ToArray();
        var items = assessmentSession.CurrentStory.Participants
            .Select(a => new AddStoryItem(a.Id.Value, a.Name, 0, AssessmentValue.None))
            .ToArray();

        return new(assessmentSession.Id.Value, assessmentSession.CurrentStory.Title, appraiserIds, items);
    }
}