using System.Net;
using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Backend.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
	private readonly CommandFactory _commandFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBuilder _messageBuilder;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
		CommandFactory commandFactory,
		IServiceProvider serviceProvider,
        IMessageBuilder messageBuilder)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        if (update.Message?.From is null || update.Message.From.IsBot)
            return;

        var userName = update.Message.From.GetUserName();
        var command = await CreateCommand(update, userName, cancellationToken);
		if (command is null)
            return;

        try
		{
			using var scope = _serviceProvider.CreateScope();
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(command, cancellationToken);
            if (result is null)
                return;

			foreach (var notification in Build((dynamic)result, update.Message.Chat.Id))
				await ProcessNotification(client, notification, userName, cancellationToken);
		}
        catch (AppraiserUserException appraiserException)
        {
            var message = _messageBuilder.Build(appraiserException.MessageId, appraiserException.Values);

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                message,
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

	private async Task<IBaseRequest?> CreateCommand(Update update, string userName, CancellationToken cancellationToken)
	{
		if (update is null)
			throw new ArgumentNullException(nameof(update));
		if (String.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

		return !string.IsNullOrWhiteSpace(update.Message!.Text)
			? await _commandFactory.Create(
				update.Message.Text,
				update.Message.Chat.Id,
				update.Message.From!.Id,
				userName,
				cancellationToken)
			: null;
	}

	private async Task ProcessNotification(
		ITelegramBotClient client,
		INotificationMessage notificationMessage,
		string userName,
		CancellationToken cancellationToken)
	{
		if (client is null)
			throw new ArgumentNullException(nameof(client));
		if (notificationMessage is null)
			throw new ArgumentNullException(nameof(notificationMessage));
		if (String.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

		var messageText = _messageBuilder.Build(notificationMessage.MessageId, notificationMessage.MessageValues);
		if (notificationMessage.TargetChatIds?.Any() == true)
			foreach (var targetChatId in notificationMessage.TargetChatIds)
			{
				var message = await client.SendTextMessageAsync(
					targetChatId,
					messageText,
					cancellationToken: cancellationToken);

				if (notificationMessage.Handler is not null)
					await notificationMessage.Handler(targetChatId, userName, message.MessageId, cancellationToken);
			}

		if (notificationMessage.TargetMessages?.Any() == true)
			foreach (var message in notificationMessage.TargetMessages)
			{
				await client.EditMessageTextAsync(
					new(message.ChatId),
					message.MessageId,
					messageText,
					cancellationToken: cancellationToken);
			}
	}

	private IEnumerable<INotificationMessage> Build<TCommandResult>(
		TCommandResult commandResult,
		long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var notificationBuilder = _serviceProvider.GetService<INotificationBuilder<TCommandResult>>();

		return notificationBuilder is not null
			? notificationBuilder.Build(commandResult, fromId)
			: Array.Empty<INotificationMessage>();
	}

	public Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message listened with error");

        return Task.CompletedTask;
    }
}