using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Common;
using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class EstimatesService : IEstimatesService
{
    public IReadOnlyCollection<EstimateItem> CreateResultByStory(Story story)
    {
        if (story is null)
            throw new ArgumentNullException(nameof(story));

        var estimateEnded = story.EstimateEnded();

        return story.StoryForEstimates
            .Select(a => new EstimateItem(
                a.Appraiser.Id.Value,
                a.Appraiser.Name,
                a.StoryExternalId,
                estimateEnded ? a.GetValue() : null,
                estimateEnded ? $"{a.GetDisplayValue()} SP" : a.Value == AssessmentValue.None ? string.Empty : "+"))
            .ToArray();
    }
}