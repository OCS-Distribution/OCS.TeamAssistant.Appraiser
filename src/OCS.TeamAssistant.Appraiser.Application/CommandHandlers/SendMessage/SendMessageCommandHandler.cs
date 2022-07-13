using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;

internal sealed class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    public Task<SendMessageResult> Handle(SendMessageCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        return Task.FromResult(new SendMessageResult(command.Text));
    }
}