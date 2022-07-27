using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.States;

internal abstract class AssessmentSessionState
{
	protected readonly IAssessmentSessionAccessor AssessmentSession;

	protected AssessmentSessionState(IAssessmentSessionAccessor assessmentSession)
		=> AssessmentSession = assessmentSession ?? throw new ArgumentNullException(nameof(assessmentSession));

	public virtual CreationStep Step => CreationStep.None;
	public virtual void Activate(ParticipantId moderatorId, string title) => Throw();
	public virtual void Connect(ParticipantId participantId, string name) => Throw();
	public virtual void StartStorySelection(ParticipantId moderatorId) => Throw();
	public virtual void StorySelected(ParticipantId moderatorId, string storyTitle) => Throw();
	public virtual void AddStoryForEstimate(StoryForEstimate storyForEstimate) => Throw();
	public virtual void Estimate(Participant participant, AssessmentValue value) => Throw();
	public virtual void CompleteEstimate(ParticipantId moderatorId) => Throw();
	public virtual void Reset(ParticipantId moderatorId) => Throw();
	public virtual void Disconnect(ParticipantId participantId) => Throw();

	private void Throw() => throw new AppraiserUserException(MessageId.NotValidState, GetType().Name);
}