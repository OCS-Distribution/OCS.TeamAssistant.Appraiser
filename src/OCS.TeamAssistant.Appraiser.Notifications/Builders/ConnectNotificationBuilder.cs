using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Connect;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ConnectNotificationBuilder : INotificationBuilder<ConnectResult>
{
	public IEnumerable<INotificationMessage> Build(ConnectResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return NotificationMessage.Create(fromId, MessageId.ConnectedSuccess, commandResult.AssessmentSessionTitle);

		if (commandResult.ChatId != fromId)
			yield return NotificationMessage.Create(
				commandResult.ChatId,
				MessageId.AppraiserAdded,
				commandResult.AppraiserName,
				commandResult.AssessmentSessionTitle);
	}
}