namespace OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;

public sealed record ShowParticipantsResult(IReadOnlyCollection<string> Appraisers);