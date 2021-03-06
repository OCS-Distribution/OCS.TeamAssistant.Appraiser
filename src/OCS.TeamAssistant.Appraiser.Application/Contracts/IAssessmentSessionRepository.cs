using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface IAssessmentSessionRepository
{
    Task<AssessmentSession?> Find(AssessmentSessionId assessmentSessionId, CancellationToken cancellationToken);

    Task<AssessmentSession?> Find(ParticipantId participantId, CancellationToken cancellationToken);

    Task Add(AssessmentSession assessmentSession, CancellationToken cancellationToken);

    Task Update(AssessmentSession assessmentSession, CancellationToken cancellationToken);

    Task Remove(AssessmentSession assessmentSession, CancellationToken cancellationToken);
}