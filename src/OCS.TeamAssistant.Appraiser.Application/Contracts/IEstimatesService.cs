using OCS.TeamAssistant.Appraiser.Application.Contracts.Common;
using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface IEstimatesService
{
    IReadOnlyCollection<EstimateItem> CreateResultByStory(Story story);
}