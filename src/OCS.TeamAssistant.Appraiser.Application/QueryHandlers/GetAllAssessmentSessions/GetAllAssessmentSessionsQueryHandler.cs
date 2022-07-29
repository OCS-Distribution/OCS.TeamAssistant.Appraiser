using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Model.Queries.GetAllAssessmentSessions;

namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.GetAllAssessmentSessions;

internal sealed class GetAllAssessmentSessionsQueryHandler
	: IRequestHandler<RequestWrapper<GetAllAssessmentSessionsQuery, GetAllAssessmentSessionsResult>, GetAllAssessmentSessionsResult>
{
	private readonly IAssessmentSessionRepository _assessmentSessionRepository;

	public GetAllAssessmentSessionsQueryHandler(IAssessmentSessionRepository assessmentSessionRepository)
	{
		_assessmentSessionRepository =
			assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
	}

	public async Task<GetAllAssessmentSessionsResult> Handle(
		RequestWrapper<GetAllAssessmentSessionsQuery, GetAllAssessmentSessionsResult> requestWrapper,
		CancellationToken cancellationToken)
	{
		if (requestWrapper is null)
			throw new ArgumentNullException(nameof(requestWrapper));

		var sessions = await _assessmentSessionRepository.GetAll(cancellationToken);

		var items = sessions
					.Select(s => new AssessmentSessionDto(s.Id.Value, s.Title))
					.ToArray();

		return new(items);
	}
}