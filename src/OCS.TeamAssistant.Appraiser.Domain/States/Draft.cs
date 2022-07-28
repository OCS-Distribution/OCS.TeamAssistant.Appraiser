using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.States;

internal sealed class Draft : AssessmentSessionState
{
	public Draft(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

	public override CreationStep Step => CreationStep.EnterTitle;

	public override void Activate(ParticipantId moderatorId, string title)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

		AssessmentSession
			.AsModerator(moderatorId)
			.ChangeTitle(title)
			.MoveToState(a => new Idle(a));
	}
}