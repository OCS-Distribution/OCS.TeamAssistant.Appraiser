using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record CreateAssessmentSessionCommand(long ChatId, long ModeratorId, string ModeratorName)
	: ICreateAssessmentSessionCommand;