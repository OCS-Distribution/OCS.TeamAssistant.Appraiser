namespace OCS.TeamAssistant.Appraiser.Domain.States;

internal interface IStoryAccessor
{
	string Title { get; }

	void AddStoryForEstimate(StoryForEstimate storyForEstimate);
	void Estimate(Participant participant, AssessmentValue value);
	void Reset();
}