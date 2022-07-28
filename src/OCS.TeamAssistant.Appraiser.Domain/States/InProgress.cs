using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.States;

internal sealed class InProgress : AssessmentSessionState
{
	public InProgress(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

	public override void Estimate(Participant participant, AssessmentValue value)
	{
		if (participant is null)
			throw new ArgumentNullException(nameof(participant));
		if (AssessmentSession.EstimateEnded())
			throw new AppraiserUserException(MessageId.EstimateRejected, AssessmentSession.Story.Title);

		AssessmentSession.Story.Estimate(participant, value);

		if (AssessmentSession.EstimateEnded())
			AssessmentSession.MoveToState(a => new Idle(a));
	}

	public override void AddStoryForEstimate(StoryForEstimate storyForEstimate)
		=> AssessmentSession.Story.AddStoryForEstimate(storyForEstimate);

	public override void CompleteEstimate(ParticipantId moderatorId)
	{
		AssessmentSession
			.AsModerator(moderatorId)
			.MoveToState(a => new Idle(a));
	}
}