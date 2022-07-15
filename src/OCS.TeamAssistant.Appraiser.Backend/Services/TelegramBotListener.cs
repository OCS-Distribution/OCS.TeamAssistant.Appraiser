using Microsoft.Extensions.Options;
using OCS.TeamAssistant.Appraiser.Backend.Options;
using Telegram.Bot;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class TelegramBotListener : IHostedService
{
    private readonly TelegramBotMessageHandler _handler;
    private readonly TelegramBotClient _client;

    public TelegramBotListener(TelegramBotMessageHandler handler, IOptions<TelegramBotOptions> options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _client = new TelegramBotClient(options.Value.AccessToken);
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _client.StartReceiving(_handler.Handle, _handler.OnError, cancellationToken: cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}