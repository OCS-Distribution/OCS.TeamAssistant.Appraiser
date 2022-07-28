using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;

internal sealed class ShowParticipantsCommandHandler : IRequestHandler<IShowParticipantsQuery, ShowParticipantsResult>
{
    private readonly IAssessmentSessionRepository _assessmentSessionRepository;

    public ShowParticipantsCommandHandler(IAssessmentSessionRepository assessmentSessionRepository)
    {
        _assessmentSessionRepository =
            assessmentSessionRepository ?? throw new ArgumentNullException(nameof(assessmentSessionRepository));
    }

    public async Task<ShowParticipantsResult> Handle(
        IShowParticipantsQuery query,
        CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var assessmentSession = await _assessmentSessionRepository
			.Find(new ParticipantId(query.ModeratorId), cancellationToken)
			.EnsureForModerator(query.ModeratorName);

		var appraisers = assessmentSession.Participants.Select(a => a.Name).ToArray();

        return new(appraisers);
    }
}