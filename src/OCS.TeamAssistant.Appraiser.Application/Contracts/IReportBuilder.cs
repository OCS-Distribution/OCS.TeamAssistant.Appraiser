using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface IReportBuilder
{
    IReadOnlyCollection<EstimateItem> Build(Story story);
}