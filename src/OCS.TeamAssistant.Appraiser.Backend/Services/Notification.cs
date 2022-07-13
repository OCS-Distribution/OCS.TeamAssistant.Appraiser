namespace OCS.TeamAssistant.Appraiser.Backend.Services;

public sealed record Notification
{
    public static readonly Notification Empty = new(
        nameof(Notification),
        targetUserIds: null,
        targetMessages: null,
        responseHandler: null);

    public string Message { get; }
    public IReadOnlyCollection<long>? TargetUserIds { get; }
    public IReadOnlyCollection<(long UserId, int MessageId)>? TargetMessages { get; }
    public Func<long, int, CancellationToken, Task>? ResponseHandler { get; }
    
    private Notification(
        string message,
        IReadOnlyCollection<long>? targetUserIds,
        IReadOnlyCollection<(long UserId, int MessageId)>? targetMessages,
        Func<long, int, CancellationToken, Task>? responseHandler)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));

        Message = message;
        TargetUserIds = targetUserIds;
        TargetMessages = targetMessages;
        ResponseHandler = responseHandler;
    }

    public static Notification Create(
        string message,
        IReadOnlyCollection<long> targetUserIds,
        Func<long, int, CancellationToken, Task>? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        if (targetUserIds is null)
            throw new ArgumentNullException(nameof(targetUserIds));
        
        return new Notification(message, targetUserIds, targetMessages: null, responseHandler);
    }
    
    public static Notification Create(
        string message,
        long targetUserId,
        Func<long, int, CancellationToken, Task>? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));

        return new Notification(message, new [] { targetUserId }, targetMessages: null, responseHandler);
    }
    
    public static Notification Edit(
        string message,
        IReadOnlyCollection<(long UserId, int MessageId)> targetMessages,
        Func<long, int, CancellationToken, Task>? responseHandler = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        if (targetMessages is null)
            throw new ArgumentNullException(nameof(targetMessages));
        
        return new Notification(message, targetUserIds: null, targetMessages, responseHandler);
    }
}