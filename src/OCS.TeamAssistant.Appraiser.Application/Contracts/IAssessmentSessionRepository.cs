using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface IAssessmentSessionRepository
{
    Task<AssessmentSession?> FindById(AssessmentSessionId assessmentSessionId, CancellationToken cancellationToken);
    
    Task<AssessmentSession?> GetByAppraiser(AppraiserId appraiserId, CancellationToken cancellationToken);
    
    Task<AssessmentSession?> FindByModerator(AppraiserId moderatorId, CancellationToken cancellationToken);

    Task Add(AssessmentSession assessmentSession, CancellationToken cancellationToken);

    Task Update(AssessmentSession assessmentSession, CancellationToken cancellationToken);
}