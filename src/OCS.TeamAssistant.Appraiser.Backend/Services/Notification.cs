namespace OCS.TeamAssistant.Appraiser.Backend.Services;

public sealed record Notification
{
    public delegate Task ResponseHandler(long chatId, string userName, int messageId, CancellationToken cancellationToken);
    
    public string Message { get; }
    public IReadOnlyCollection<long>? TargetChatIds { get; }
    public IReadOnlyCollection<(long ChatId, int MessageId)>? TargetMessages { get; }
    public ResponseHandler? Handler { get; }

    private Notification(
        string message,
        IReadOnlyCollection<long>? targetChatIds,
        IReadOnlyCollection<(long UserId, int MessageId)>? targetMessages,
        ResponseHandler? handler)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));

        Message = message;
        TargetChatIds = targetChatIds;
        TargetMessages = targetMessages;
        Handler = handler;
    }

    public static Notification Create(
        string message,
        IReadOnlyCollection<long> targetChatIds,
        ResponseHandler? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        if (targetChatIds is null)
            throw new ArgumentNullException(nameof(targetChatIds));

        return new Notification(message, targetChatIds, targetMessages: null, responseHandler);
    }

    public static Notification Create(
        string message,
        long targetChatId,
        ResponseHandler? responseHandler = null)
        => Create(message, new[] { targetChatId }, responseHandler);

    public static Notification Edit(
        string message,
        IReadOnlyCollection<(long ChatId, int MessageId)> targetMessages,
        ResponseHandler? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        if (targetMessages is null)
            throw new ArgumentNullException(nameof(targetMessages));

        return new Notification(message, targetChatIds: null, targetMessages, responseHandler);
    }
}