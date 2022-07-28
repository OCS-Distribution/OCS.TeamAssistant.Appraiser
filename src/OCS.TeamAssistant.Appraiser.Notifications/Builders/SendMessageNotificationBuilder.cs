using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class SendMessageNotificationBuilder : INotificationBuilder<SendMessageResult>
{
	public IEnumerable<INotificationMessage> Build(SendMessageResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return NotificationMessage.Create(fromId, new(commandResult.Text));
	}
}