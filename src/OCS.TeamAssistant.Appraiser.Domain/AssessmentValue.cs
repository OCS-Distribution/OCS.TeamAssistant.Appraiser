namespace OCS.TeamAssistant.Appraiser.Domain;

public enum AssessmentValue
{
    Unknown = -1,
    None = 0,
    Sp1 = 1,
    Sp2 = 2,
    Sp3 = 3,
    Sp5 = 5,
    Sp8 = 8,
    Sp13 = 13,
    Sp21 = 21
}

public static class AssessmentValueRules
{
	public static readonly IReadOnlyCollection<AssessmentValue> GetAssessments = Enum.GetValues<AssessmentValue>()
		.Where(i => i > 0)
		.ToArray();

	public static AssessmentValue ToAssessmentValue(this int? assessment)
	{
		if (assessment.HasValue && Enum.IsDefined(typeof(AssessmentValue), assessment.Value))
			return (AssessmentValue) assessment.Value;

		return AssessmentValue.Unknown;
	}
}