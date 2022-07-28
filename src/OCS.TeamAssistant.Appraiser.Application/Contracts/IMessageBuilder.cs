using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface IMessageBuilder
{
    string Build(MessageId messageId, params object[] values);
}