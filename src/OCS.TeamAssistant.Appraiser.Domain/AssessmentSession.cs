using OCS.TeamAssistant.Appraiser.Domain.States;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class AssessmentSession : IAssessmentSessionAccessor
{
    public AssessmentSessionId Id { get; }
    public long ChatId { get; }
    public Participant Moderator { get; }
    public LanguageId LanguageId { get; }
    public string Title { get; private set; }
    internal AssessmentSessionState State { get; private set; }
	public Story CurrentStory { get; private set; }
	IStoryAccessor IAssessmentSessionAccessor.Story => CurrentStory;

    private readonly List<Participant> _participants;
    public IReadOnlyCollection<Participant> Participants => _participants;
	public CreationStep CreationStep => State.Step;

    public AssessmentSession(long chatId, Participant moderator)
	{
		Moderator = moderator ?? throw new ArgumentNullException(nameof(moderator));
		Id = new(Guid.NewGuid());
		ChatId = chatId;
		_participants = new() { moderator };
        LanguageId = LanguageId.Default;
		State = new Draft(this);
		CurrentStory = Story.Empty;
		Title = string.Empty;
	}

	public bool EstimateEnded()
		=> _participants.Count == CurrentStory.StoryForEstimates.Count(s => s.Value != AssessmentValue.None);

	public void Activate(ParticipantId moderatorId, string title) => State.Activate(moderatorId, title);
	public void Connect(ParticipantId participantId, string name) => State.Connect(participantId, name);
	public void StartStorySelection(ParticipantId moderatorId) => State.StartStorySelection(moderatorId);
	public void StorySelected(ParticipantId moderatorId, string storyTitle)
		=> State.StorySelected(moderatorId, storyTitle);
	public void AddStoryForEstimate(StoryForEstimate storyForEstimate) => State.AddStoryForEstimate(storyForEstimate);
	public void Estimate(Participant participant, AssessmentValue value) => State.Estimate(participant, value);
	public void CompleteEstimate(ParticipantId moderatorId) => State.CompleteEstimate(moderatorId);
	public void Disconnect(ParticipantId participantId) => State.Disconnect(participantId);
	public void Reset(ParticipantId moderatorId) => State.Reset(moderatorId);

	IAssessmentSessionAccessor IAssessmentSessionAccessor.AddParticipant(Participant participant)
	{
		if (participant is null)
			throw new ArgumentNullException(nameof(participant));

		_participants.Add(participant);

		return this;
	}

	IAssessmentSessionAccessor IAssessmentSessionAccessor.RemoveParticipant(Participant participant)
	{
		if (participant is null)
			throw new ArgumentNullException(nameof(participant));

		_participants.Remove(participant);

		return this;
	}

	IAssessmentSessionAccessor IAssessmentSessionAccessor.AsModerator(ParticipantId participantId)
	{
		if (participantId is null)
			throw new ArgumentNullException(nameof(participantId));
		if (!Moderator.Id.Equals(participantId))
			throw new AppraiserUserException(MessageId.NoRightsAddTaskToSession, Title);

		return this;
	}

	IAssessmentSessionAccessor IAssessmentSessionAccessor.ChangeTitle(string title)
	{
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

		Title = title;

		return this;
	}

	IAssessmentSessionAccessor IAssessmentSessionAccessor.ChangeStory(string storyTitle)
	{
		if (String.IsNullOrWhiteSpace(storyTitle))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));

		CurrentStory = new(storyTitle, Participants);

		return this;
	}

	void IAssessmentSessionAccessor.MoveToState(Func<IAssessmentSessionAccessor, AssessmentSessionState> nextStateFactory)
	{
		if (nextStateFactory is null)
			throw new ArgumentNullException(nameof(nextStateFactory));

		State = nextStateFactory(this);
	}
}