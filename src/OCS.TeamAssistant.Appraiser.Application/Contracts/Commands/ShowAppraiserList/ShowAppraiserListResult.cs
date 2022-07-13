namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;

public sealed record ShowAppraiserListResult(IReadOnlyCollection<string> Appraisers);