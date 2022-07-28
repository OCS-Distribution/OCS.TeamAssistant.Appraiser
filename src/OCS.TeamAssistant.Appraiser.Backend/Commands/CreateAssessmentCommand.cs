using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessment;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record CreateAssessmentCommand(long ChatId, long ModeratorId, string ModeratorName)
	: ICreateAssessmentCommand;