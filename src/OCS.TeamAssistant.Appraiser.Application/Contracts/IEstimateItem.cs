using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface IEstimateItem
{
	long AppraiserId { get; }
	string AppraiserName { get; }
	int StoryExternalId { get; }
	AssessmentValue Value { get; }
}