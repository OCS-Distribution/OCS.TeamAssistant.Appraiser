namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectAppraiser;

public sealed record ConnectAppraiserResult(long ChatId, string AssessmentSessionTitle, string AppraiserName);