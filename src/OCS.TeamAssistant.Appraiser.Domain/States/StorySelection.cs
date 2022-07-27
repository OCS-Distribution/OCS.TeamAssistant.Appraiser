using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.States;

internal sealed class StorySelection : AssessmentSessionState
{
	public StorySelection(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

	public override CreationStep Step => CreationStep.EnterStory;

	public override void StorySelected(ParticipantId moderatorId, string storyTitle)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));
		if (string.IsNullOrWhiteSpace(storyTitle))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));

		AssessmentSession
			  .AsModerator(moderatorId)
			  .ChangeStory(storyTitle)
			  .MoveToState(a => new InProgress(a));
	}
}