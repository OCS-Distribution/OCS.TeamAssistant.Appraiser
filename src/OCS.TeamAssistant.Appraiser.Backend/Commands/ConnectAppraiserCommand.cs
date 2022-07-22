using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectAppraiser;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record ConnectAppraiserCommand(Guid AssessmentSessionId, long AppraiserId, string AppraiserName)
	: IConnectAppraiserCommand;