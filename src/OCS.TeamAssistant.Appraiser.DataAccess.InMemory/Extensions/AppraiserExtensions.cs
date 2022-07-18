using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.DataAccess.InMemory.Extensions;

internal static class AppraiserExtensions
{
    public static Domain.Appraiser GetAppraiserById(
        this IEnumerable<AssessmentSession> assessmentSessions,
        AppraiserId appraiserId)
    {
        if (assessmentSessions is null)
            throw new ArgumentNullException(nameof(assessmentSessions));
        if (appraiserId is null)
            throw new ArgumentNullException(nameof(appraiserId));

        var appraiser = assessmentSessions.SelectMany(a => a.Appraisers).FirstOrDefault(a => a.Id == appraiserId);

        if (appraiser is null)
            throw new AppraiserException($"Участник {appraiserId.Value} не найден.");
        
        return appraiser;
    }
}