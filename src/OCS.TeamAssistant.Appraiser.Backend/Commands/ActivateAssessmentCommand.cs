using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record ActivateAssessmentCommand(long ModeratorId, string ModeratorName, string Title)
	: IActivateAssessmentCommand;