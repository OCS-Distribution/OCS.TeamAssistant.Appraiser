using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record EndAssessmentSessionCommand(long ModeratorId, string ModeratorName)
	: IEndAssessmentSessionCommand;