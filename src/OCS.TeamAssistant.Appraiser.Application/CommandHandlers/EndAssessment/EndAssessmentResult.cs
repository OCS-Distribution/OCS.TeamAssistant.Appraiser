namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessment;

public sealed record EndAssessmentResult(string AssessmentSessionTitle, IReadOnlyCollection<long> AppraiserIds);