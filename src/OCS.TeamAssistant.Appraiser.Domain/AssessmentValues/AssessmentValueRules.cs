namespace OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;

public static class AssessmentValueRules
{
    public static readonly IReadOnlyCollection<AssessmentValue> GetAssessments = Enum.GetValues<AssessmentValue>()
        .Where(i => i > 0)
        .ToArray();
}