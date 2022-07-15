namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;

public sealed record EndAssessmentSessionResult(string AssessmentSessionTitle, IReadOnlyCollection<long> AppraiserIds);