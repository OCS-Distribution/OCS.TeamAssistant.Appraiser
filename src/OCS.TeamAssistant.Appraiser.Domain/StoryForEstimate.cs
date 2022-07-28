namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class StoryForEstimate
{
    public Participant Participant { get; }
    public int StoryExternalId { get; }
    public AssessmentValue Value { get; private set; }

    public StoryForEstimate(Participant participant, int storyExternalId)
    {
		Participant = participant ?? throw new ArgumentNullException(nameof(participant));
		StoryExternalId = storyExternalId;
		Value = AssessmentValue.None;
	}

	public void SetValue(AssessmentValue value) => Value = value;

	internal void Reset() => Value = AssessmentValue.None;
}