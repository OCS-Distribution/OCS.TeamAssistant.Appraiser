namespace OCS.TeamAssistant.Appraiser.Backend;

public sealed class TelegramBotOptions
{
    public string AccessToken { get; set; } = default!;
    public string LinkTemplate { get; set; } = default!;
}