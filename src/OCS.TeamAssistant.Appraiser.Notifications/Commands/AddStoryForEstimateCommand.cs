using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;

namespace OCS.TeamAssistant.Appraiser.Notifications.Commands;

internal sealed record AddStoryForEstimateCommand(
	Guid AssessmentSessionId,
	long AppraiserId,
	string AppraiserName,
	int StoryExternalId)
	: IAddStoryForEstimateCommand;