using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Disconnect;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record DisconnectCommand(long AppraiserId, string AppraiserName) : IDisconnectCommand;