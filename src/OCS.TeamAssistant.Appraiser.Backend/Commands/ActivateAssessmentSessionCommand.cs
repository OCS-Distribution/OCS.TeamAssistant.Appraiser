using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessmentSession;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record ActivateAssessmentSessionCommand(long ModeratorId, string ModeratorName, string Title)
	: IActivateAssessmentSessionCommand;