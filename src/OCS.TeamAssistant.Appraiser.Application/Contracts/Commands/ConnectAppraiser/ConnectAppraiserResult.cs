namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;

public sealed record ConnectAppraiserResult(long ChatId, string AssessmentSessionTitle, string AppraiserName);