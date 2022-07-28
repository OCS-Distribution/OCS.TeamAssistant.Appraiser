using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record SendMessageCommand(string Text) : ISendMessageCommand;