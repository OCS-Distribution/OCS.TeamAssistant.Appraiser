using System.Net.Http.Json;
using OCS.TeamAssistant.Appraiser.Model;
using OCS.TeamAssistant.Appraiser.Model.Queries.GetAllAssessmentSessions;
using OCS.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

namespace OCS.TeamAssistant.Appraiser.Dashboard.Services;

internal sealed class AssessmentSessionsService : IAssessmentSessionsService
{
	private readonly HttpClient _client;

	public AssessmentSessionsService(HttpClient client)
		=> _client = client ?? throw new ArgumentNullException(nameof(client));

	public async Task<GetAllAssessmentSessionsResult> GetAll()
	{
		var result = await _client.GetFromJsonAsync<GetAllAssessmentSessionsResult>("sessions/list");

		if (result is null)
			throw new ApplicationException("Response is null");

		return result;
	}

	public async Task<GetStoryDetailsResult> GetStoryDetails(Guid assessmentSessionId)
	{
		var result = await _client.GetFromJsonAsync<GetStoryDetailsResult>($"sessions/story/{assessmentSessionId}");

		if (result is null)
			throw new ApplicationException("Response is null");

		return result;
	}
}