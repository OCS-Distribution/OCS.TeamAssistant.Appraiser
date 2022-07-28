using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.States;

internal sealed class Idle : AssessmentSessionState
{
	public Idle(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

	public override void Connect(ParticipantId participantId, string name)
	{
		if (participantId is null)
			throw new ArgumentNullException(nameof(participantId));
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

		if (AssessmentSession.Participants.Any(p => p.Id == participantId))
			throw new AppraiserUserException(MessageId.AppraiserConnectWithError, name, AssessmentSession.Title);

		AssessmentSession.AddParticipant(new(participantId, name));
	}

	public override void Disconnect(ParticipantId participantId)
	{
		if (participantId is null)
			throw new ArgumentNullException(nameof(participantId));

		var appraiser = AssessmentSession.Participants.SingleOrDefault(a => a.Id == participantId);

		if (appraiser is null)
			throw new AppraiserUserException(MessageId.ShutdownCompletedWithError, AssessmentSession.Title);

		if (AssessmentSession.Moderator.Id == appraiser.Id)
			throw new AppraiserUserException(MessageId.ModeratorCannotDisconnectedFromSession, AssessmentSession.Title);

		AssessmentSession.RemoveParticipant(appraiser);
	}

	public override void StartStorySelection(ParticipantId moderatorId)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));

		AssessmentSession
			.AsModerator(moderatorId)
			.MoveToState(a => new StorySelection(a));
	}

	public override void Reset(ParticipantId moderatorId)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));

		AssessmentSession
			.AsModerator(moderatorId)
			.Story.Reset();

		AssessmentSession.MoveToState(a => new InProgress(a));
	}
}