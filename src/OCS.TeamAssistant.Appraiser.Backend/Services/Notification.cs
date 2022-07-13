namespace OCS.TeamAssistant.Appraiser.Backend.Services;

public sealed record Notification
{
    public static readonly Notification Empty = new(
        nameof(Notification),
        targetChatIds: null,
        targetMessages: null,
        responseHandler: null);

    public string Message { get; }
    public IReadOnlyCollection<long>? TargetChatIds { get; }
    public IReadOnlyCollection<(long ChatId, int MessageId)>? TargetMessages { get; }
    public Func<long, int, CancellationToken, Task>? ResponseHandler { get; }
    
    private Notification(
        string message,
        IReadOnlyCollection<long>? targetChatIds,
        IReadOnlyCollection<(long UserId, int MessageId)>? targetMessages,
        Func<long, int, CancellationToken, Task>? responseHandler)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));

        Message = message;
        TargetChatIds = targetChatIds;
        TargetMessages = targetMessages;
        ResponseHandler = responseHandler;
    }

    public static Notification Create(
        string message,
        IReadOnlyCollection<long> targetChatIds,
        Func<long, int, CancellationToken, Task>? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        if (targetChatIds is null)
            throw new ArgumentNullException(nameof(targetChatIds));
        
        return new Notification(message, targetChatIds, targetMessages: null, responseHandler);
    }
    
    public static Notification Create(
        string message,
        long targetChatIds,
        Func<long, int, CancellationToken, Task>? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));

        return new Notification(message, new [] { targetChatIds }, targetMessages: null, responseHandler);
    }
    
    public static Notification Edit(
        string message,
        IReadOnlyCollection<(long ChatId, int MessageId)> targetMessages,
        Func<long, int, CancellationToken, Task>? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        if (targetMessages is null)
            throw new ArgumentNullException(nameof(targetMessages));
        
        return new Notification(message, targetChatIds: null, targetMessages, responseHandler);
    }
}