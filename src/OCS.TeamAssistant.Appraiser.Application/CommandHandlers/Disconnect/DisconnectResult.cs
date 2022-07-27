namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Disconnect;

public sealed record DisconnectResult(long ChatId, string AssessmentSessionTitle, string AppraiserName);