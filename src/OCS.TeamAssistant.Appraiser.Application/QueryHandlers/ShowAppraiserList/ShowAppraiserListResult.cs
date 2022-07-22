namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowAppraiserList;

public sealed record ShowAppraiserListResult(IReadOnlyCollection<string> Appraisers);