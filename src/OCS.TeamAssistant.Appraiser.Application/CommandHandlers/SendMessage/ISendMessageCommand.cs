using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;

public interface ISendMessageCommand : IRequest<SendMessageResult>
{
	string Text { get; }
}