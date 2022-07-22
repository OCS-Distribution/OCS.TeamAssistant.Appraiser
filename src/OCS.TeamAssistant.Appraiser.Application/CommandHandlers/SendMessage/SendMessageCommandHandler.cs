using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;

internal sealed class SendMessageCommandHandler : IRequestHandler<ISendMessageCommand, SendMessageResult>
{
    public Task<SendMessageResult> Handle(ISendMessageCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        return Task.FromResult(new SendMessageResult(command.Text));
    }
}