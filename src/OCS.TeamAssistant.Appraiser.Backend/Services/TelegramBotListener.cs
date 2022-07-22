using Telegram.Bot;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class TelegramBotListener : IHostedService
{
    private readonly TelegramBotMessageHandler _handler;
    private readonly TelegramBotClient _client;

    public TelegramBotListener(TelegramBotMessageHandler handler, string accessToken)
    {
		if (handler == null)
			throw new ArgumentNullException(nameof(handler));
		if (String.IsNullOrWhiteSpace(accessToken))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));

		_handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _client = new(accessToken);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _client.StartReceiving(_handler.Handle, _handler.OnError, cancellationToken: cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}