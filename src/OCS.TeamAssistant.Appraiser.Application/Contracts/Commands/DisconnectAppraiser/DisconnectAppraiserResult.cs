namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.DisconnectAppraiser;

public sealed record DisconnectAppraiserResult(long ChatId, string AssessmentSessionTitle, string AppraiserName);