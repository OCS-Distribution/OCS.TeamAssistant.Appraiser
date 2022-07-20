using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.Exceptions;

public sealed class AppraiserException : ApplicationException
{
    public MessageId MessageId { get; }
    public object[] Values { get; }

    public AppraiserException(MessageId messageId, params object[] values)
        : base(messageId.Value)
    {
        MessageId = messageId;
        Values = values;
    }
}