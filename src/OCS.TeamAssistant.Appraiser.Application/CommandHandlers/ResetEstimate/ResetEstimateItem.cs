using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;

public sealed record ResetEstimateItem(
	long AppraiserId,
	string AppraiserName,
	int StoryExternalId,
	AssessmentValue Value)
	: IEstimateItem;