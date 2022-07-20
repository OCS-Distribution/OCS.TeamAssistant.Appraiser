using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

public sealed class Notification
{
    public delegate Task ResponseHandler(long chatId, string userName, int messageId, CancellationToken cancellationToken);
    
    public MessageId MessageId { get; }
    public object[] MessageValues { get; }
    public IReadOnlyCollection<long>? TargetChatIds { get; }
    public IReadOnlyCollection<(long ChatId, int MessageId)>? TargetMessages { get; }
    public ResponseHandler? Handler { get; private set; }

    private Notification(
        IReadOnlyCollection<long>? targetChatIds,
        IReadOnlyCollection<(long UserId, int MessageId)>? targetMessages,
        MessageId messageId,
        object[] messageValues)
    {
        MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
        MessageValues = messageValues ?? throw new ArgumentNullException(nameof(messageValues));
        TargetChatIds = targetChatIds;
        TargetMessages = targetMessages;
    }

    public Notification AddHandler(ResponseHandler handler)
    {
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));

        return this;
    }

    public static Notification Create(
        IReadOnlyCollection<long> targetChatIds,
        MessageId messageId,
        params object[] messageValues)
    {
        if (targetChatIds is null)
            throw new ArgumentNullException(nameof(targetChatIds));
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));
        if (messageValues is null)
            throw new ArgumentNullException(nameof(messageValues));

        return new Notification(targetChatIds, targetMessages: null, messageId, messageValues);
    }

    public static Notification Create(long targetChatId, MessageId messageId, params object[] messageValues)
        => Create(new[] { targetChatId }, messageId, messageValues);

    public static Notification Edit(
        IReadOnlyCollection<(long ChatId, int MessageId)> targetMessages,
        MessageId messageId,
        params object[] messageValues)
    {
        if (targetMessages is null)
            throw new ArgumentNullException(nameof(targetMessages));
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));
        if (messageValues is null)
            throw new ArgumentNullException(nameof(messageValues));

        return new Notification(targetChatIds: null, targetMessages, messageId, messageValues);
    }
}