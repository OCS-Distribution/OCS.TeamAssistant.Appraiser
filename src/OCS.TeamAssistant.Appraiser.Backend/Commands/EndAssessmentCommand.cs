using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessment;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record EndAssessmentCommand(long ModeratorId, string ModeratorName)
	: IEndAssessmentCommand;