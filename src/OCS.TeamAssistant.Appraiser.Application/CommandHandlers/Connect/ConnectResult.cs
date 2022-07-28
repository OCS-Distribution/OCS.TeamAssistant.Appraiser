namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Connect;

public sealed record ConnectResult(long ChatId, string AssessmentSessionTitle, string AppraiserName);