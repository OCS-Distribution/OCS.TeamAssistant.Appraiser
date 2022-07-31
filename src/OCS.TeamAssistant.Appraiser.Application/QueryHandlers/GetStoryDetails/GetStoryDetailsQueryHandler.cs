using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.GetStoryDetails;

internal sealed class GetStoryDetailsQueryHandler
	: IRequestHandler<RequestWrapper<GetStoryDetailsQuery, GetStoryDetailsResult>, GetStoryDetailsResult>
{
	private readonly IAssessmentSessionRepository _assessmentSessionRepository;

	public GetStoryDetailsQueryHandler(IAssessmentSessionRepository assessmentSessionRepository)
	{
		_assessmentSessionRepository =
			assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
	}

	public async Task<GetStoryDetailsResult> Handle(
		RequestWrapper<GetStoryDetailsQuery, GetStoryDetailsResult> queryWrapper,
		CancellationToken cancellationToken)
	{
		if (queryWrapper is null)
			throw new ArgumentNullException(nameof(queryWrapper));

		var assessmentSession = await _assessmentSessionRepository.Find(
			new AssessmentSessionId(queryWrapper.Request.AssessmentSessionId),
			cancellationToken);

		if (assessmentSession is null)
			throw new AppraiserException($"AssessmentSession {queryWrapper.Request.AssessmentSessionId} was not found");

		if (assessmentSession.CurrentStory == Story.Empty)
			return GetStoryDetailsResult.Empty;

		var estimateEnded = assessmentSession.EstimateEnded();
		var items = assessmentSession.CurrentStory.StoryForEstimates
			.Select(e => new StoryForEstimateDto(
				e.Participant.Name,
				estimateEnded ? e.Value.DisplayValue() : e.Value.DisplayHasValue()))
			.ToArray();

		return new(
			assessmentSession.Title,
			assessmentSession.CurrentStory.Title,
			items,
			assessmentSession.CurrentStory.GetTotal().DisplayValue());
	}
}