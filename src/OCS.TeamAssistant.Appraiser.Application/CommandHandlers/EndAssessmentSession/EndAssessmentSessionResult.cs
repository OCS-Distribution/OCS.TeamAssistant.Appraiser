namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;

public sealed record EndAssessmentSessionResult(string AssessmentSessionTitle, IReadOnlyCollection<long> AppraiserIds);