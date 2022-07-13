using MediatR;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly IMediator _mediator;
    private readonly CommandFactory _commandFactory;
    private readonly CommandResultProcessor _commandResultProcessor;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        IMediator mediator,
        CommandFactory commandFactory,
        CommandResultProcessor commandResultProcessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandResultProcessor =
            commandResultProcessor ?? throw new ArgumentNullException(nameof(commandResultProcessor));
    }

    public async Task Handle(ITelegramBotClient client, Update message, CancellationToken cancellationToken)
    {
        if (message.Message is null || message.Message.From is null || message.Message.From.IsBot)
            return;
        
        var command = message.Message.From?.Username is not null && !string.IsNullOrWhiteSpace(message.Message.Text)
            ? await _commandFactory.Create(
                message.Message.Text,
                message.Message.From.Id,
                message.Message.From.Username,
                cancellationToken)
            : _commandFactory.CreateErrorHandleCommand();

        try
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result is not null)
            {
                var response = _commandResultProcessor.Process(result, message.Message.From!.Id);

                if (Notification.Empty.Equals(response))
                    return;

                foreach (var userId in response.UserIds)
                    await client.SendTextMessageAsync(userId, response.Message, cancellationToken: cancellationToken);
            }
        }
        catch (AppraiserException appraiserException)
        {
            await client.SendTextMessageAsync(
                message.Message.Chat.Id,
                appraiserException.Message,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
            
            await client.SendTextMessageAsync(
                message.Message.Chat.Id,
                "Возникло необработанное исключение. Попробуйте выполнить команду повторно.",
                cancellationToken: cancellationToken);
        }
    }

    public Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message listened with error");
        
        return Task.CompletedTask;
    }
}