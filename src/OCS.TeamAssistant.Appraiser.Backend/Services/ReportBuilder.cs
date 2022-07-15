using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class ReportBuilder : IReportBuilder
{
    public IReadOnlyCollection<EstimateItem> Build(Story story)
    {
        if (story is null)
            throw new ArgumentNullException(nameof(story));

        var estimateEnded = story.EstimateEnded();

        return story.StoryForEstimates
            .Select(a =>
            {
                var valueForDisplay = estimateEnded ? ForDisplayResult(a) : ForDisplay(a);

                return new EstimateItem(
                    a.Appraiser.Id.Value,
                    a.Appraiser.Name,
                    a.StoryExternalId,
                    valueForDisplay.Value,
                    valueForDisplay.DisplayValue);
            })
            .ToArray();
    }

    private static (int? Value, string DisplayValue) ForDisplayResult(StoryForEstimate storyForEstimate)
    {
        if (storyForEstimate is null)
            throw new ArgumentNullException(nameof(storyForEstimate));

        var value = storyForEstimate.GetValue();
        var displayValue = storyForEstimate.Value switch
        {
            AssessmentValue.None => "-",
            AssessmentValue.Unknown => "?",
            _ => $"{value} SP"
        };

        return (value, displayValue);
    }

    private static (int? Value, string DisplayValue) ForDisplay(StoryForEstimate storyForEstimate)
    {
        if (storyForEstimate is null)
            throw new ArgumentNullException(nameof(storyForEstimate));

        var displayValue = storyForEstimate.Value == AssessmentValue.None ? string.Empty : "+";
        
        return (default, displayValue);
    }
}