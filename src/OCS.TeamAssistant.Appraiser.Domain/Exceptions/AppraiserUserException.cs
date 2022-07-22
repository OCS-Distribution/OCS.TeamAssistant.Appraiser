using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain.Exceptions;

public sealed class AppraiserUserException : AppraiserException
{
    public MessageId MessageId { get; }
    public object[] Values { get; }

    public AppraiserUserException(MessageId messageId, params object[] values)
        : base(messageId.Value)
    {
        MessageId = messageId;
        Values = values;
    }
}