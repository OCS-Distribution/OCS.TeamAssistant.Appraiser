using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record DisconnectAppraiserCommand(long AppraiserId, string AppraiserName) : IDisconnectAppraiserCommand;