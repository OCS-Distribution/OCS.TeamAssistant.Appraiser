namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;

public sealed record EstimateItem(long AppraiserId, string AppraiserName, int StoryExternalId, bool Exists);