using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowAppraiserList;

internal sealed class ShowAppraiserListCommandHandler
    : IRequestHandler<IShowAppraiserListQuery, ShowAppraiserListResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ShowAppraiserListCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<ShowAppraiserListResult> Handle(
        IShowAppraiserListQuery query,
        CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var assessmentSession = await _assessmentSessionRepository
			.Find(new AppraiserId(query.ModeratorId), cancellationToken)
			.EnsureForModerator(AssessmentSessionState.Active, query.ModeratorName);

		var appraisers = assessmentSession.Appraisers.Select(a => a.Name).ToArray();

        return new(appraisers);
    }
}