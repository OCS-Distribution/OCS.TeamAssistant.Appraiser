using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.States;

internal interface IAssessmentSessionAccessor
{
	string Title { get; }
	Participant Moderator { get; }
	IReadOnlyCollection<Participant> Participants { get; }
	IStoryAccessor Story { get; }

	bool EstimateEnded();
	IAssessmentSessionAccessor AsModerator(ParticipantId moderatorId);
	IAssessmentSessionAccessor ChangeTitle(string title);
	IAssessmentSessionAccessor AddParticipant(Participant participant);
	IAssessmentSessionAccessor RemoveParticipant(Participant participant);
	IAssessmentSessionAccessor ChangeStory(string title);
	void MoveToState(Func<IAssessmentSessionAccessor, AssessmentSessionState> nextStateFactory);
}