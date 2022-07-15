namespace OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;

public static class AssessmentValueRules
{
    public static readonly AssessmentValue[] GetAssessments = Enum.GetValues<AssessmentValue>()
        .Where(i => i > 0)
        .ToArray();
}