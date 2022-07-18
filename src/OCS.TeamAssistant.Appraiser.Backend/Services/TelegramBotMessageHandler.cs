using System.Net;
using MediatR;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
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

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (update.Message?.From is null || update.Message.From.IsBot)
            return;

        var userName = GetName(update.Message.From);
        var command = !string.IsNullOrWhiteSpace(update.Message.Text)
            ? await _commandFactory.Create(
                update.Message.Text,
                update.Message.Chat.Id,
                update.Message.From.Id,
                userName,
                cancellationToken)
            : null;

        if (command is null)
            return;
        
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result is null)
                return;

            var responses = _commandResultProcessor.Process(result, update.Message.Chat.Id);
            foreach (var response in responses)
            {
                if (response.TargetChatIds?.Any() == true)
                    foreach (var targetChatId in response.TargetChatIds)
                    {
                        var message = await client.SendTextMessageAsync(
                            targetChatId,
                            response.Message,
                            cancellationToken: cancellationToken);

                        if (response.Handler is not null)
                            await response.Handler(targetChatId, userName, message.MessageId, cancellationToken);
                    }
                if (response.TargetMessages?.Any() == true)
                    foreach (var message in response.TargetMessages)
                    {
                        await client.EditMessageTextAsync(
                            new ChatId(message.ChatId),
                            message.MessageId,
                            response.Message,
                            cancellationToken: cancellationToken);
                    }
            }
        }
        catch (AppraiserException appraiserException)
        {
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                appraiserException.Message,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException apiRequestException)
            when (apiRequestException.ErrorCode == (int)HttpStatusCode.BadRequest)
        {
            // ignore
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
            
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                "Возникло необработанное исключение. Попробуйте выполнить команду повторно.",
                cancellationToken: cancellationToken);
        }
    }

    public Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message listened with error");
        
        return Task.CompletedTask;
    }
    
    private string GetName(User user) => user.Username
        ?? (string.IsNullOrWhiteSpace(user.LastName) ? user.FirstName : $"{user.FirstName} {user.LastName}");
}