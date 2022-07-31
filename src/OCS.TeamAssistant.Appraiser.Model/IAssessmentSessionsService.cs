using OCS.TeamAssistant.Appraiser.Model.Queries.GetAllAssessmentSessions;
using OCS.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

namespace OCS.TeamAssistant.Appraiser.Model;

public interface IAssessmentSessionsService
{
	Task<GetAllAssessmentSessionsResult> GetAll();

	Task<GetStoryDetailsResult> GetStoryDetails(Guid assessmentSessionId);
}