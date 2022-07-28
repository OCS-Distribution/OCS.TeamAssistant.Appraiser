using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface INotificationMessage
{
	delegate Task ResponseHandler(long chatId, string userName, int messageId, CancellationToken cancellationToken);

	MessageId MessageId { get; }
	object[] MessageValues { get; }
	IReadOnlyCollection<long>? TargetChatIds { get; }
	IReadOnlyCollection<(long ChatId, int MessageId)>? TargetMessages { get; }
	ResponseHandler? Handler { get; }
}