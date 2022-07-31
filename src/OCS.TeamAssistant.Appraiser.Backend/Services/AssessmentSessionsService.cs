using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Model;
using OCS.TeamAssistant.Appraiser.Model.Queries.GetAllAssessmentSessions;
using OCS.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class AssessmentSessionsService : IAssessmentSessionsService
{
	private readonly IMediator _mediator;

	public AssessmentSessionsService(IMediator mediator)
		=> _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

	public async Task<GetAllAssessmentSessionsResult> GetAll()
	{
		var request = new RequestWrapper<GetAllAssessmentSessionsQuery, GetAllAssessmentSessionsResult>(new());
		return await _mediator.Send(request);
	}

	public async Task<GetStoryDetailsResult> GetStoryDetails(Guid assessmentSessionId)
	{
		var request = new RequestWrapper<GetStoryDetailsQuery, GetStoryDetailsResult>(new(assessmentSessionId));
		return await _mediator.Send(request);
	}
}