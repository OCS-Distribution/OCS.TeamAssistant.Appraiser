using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Notifications.Extensions;

internal static class AssessmentValueExtensions
{
	public static string DisplayValue(this AssessmentValue value)
	{
		return value switch
		{
			AssessmentValue.Unknown => "?",
			AssessmentValue.None => "-",
			_ => ((int)value).ToString()
		};
	}

	public static string DisplayValue(this decimal? value) => value?.ToString(".##") ?? "?";

	public static string DisplayHasValue(this AssessmentValue value) => value == AssessmentValue.None
		? "-"
		: "+";
}