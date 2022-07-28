using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Connect;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record ConnectCommand(Guid AssessmentSessionId, long AppraiserId, string AppraiserName)
	: IConnectCommand;