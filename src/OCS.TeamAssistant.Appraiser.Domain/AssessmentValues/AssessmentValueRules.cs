namespace OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;

public static class AssessmentValueRules
{
    public static readonly string[] GetAssessments = Enum.GetValues<AssessmentValue>()
        .Where(i => i > 0)
        .Select(i => ((int)i).ToString())
        .ToArray();
}