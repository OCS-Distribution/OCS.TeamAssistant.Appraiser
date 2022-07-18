using OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public sealed record EstimateItem(long AppraiserId, string AppraiserName, int StoryExternalId, AssessmentValue Value);