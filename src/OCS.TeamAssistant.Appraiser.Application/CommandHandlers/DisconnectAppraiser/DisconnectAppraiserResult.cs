namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;

public sealed record DisconnectAppraiserResult(long ChatId, string AssessmentSessionTitle, string AppraiserName);