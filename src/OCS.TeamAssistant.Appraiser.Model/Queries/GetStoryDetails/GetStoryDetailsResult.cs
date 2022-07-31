namespace OCS.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

public sealed record GetStoryDetailsResult(
	string AssessmentSessionTitle,
	string StoryTitle,
	IReadOnlyCollection<StoryForEstimateDto> StoryForEstimates,
	string? Total)
{
	public static readonly GetStoryDetailsResult Empty = new(
		string.Empty,
		string.Empty,
		Array.Empty<StoryForEstimateDto>(),
		Total: null);
}