using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Models;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class DisconnectAppraiserNotificationBuilder : INotificationBuilder<DisconnectAppraiserResult>
{
	public IEnumerable<INotificationMessage> Build(DisconnectAppraiserResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return NotificationMessage.Create(
			fromId,
			MessageId.DisconnectedFromSession,
			commandResult.AssessmentSessionTitle);

		if (commandResult.ChatId != fromId)
			yield return NotificationMessage.Create(
				commandResult.ChatId,
				MessageId.AppraiserDisconnectedFromSession,
				commandResult.AppraiserName,
				commandResult.AssessmentSessionTitle);
	}
}