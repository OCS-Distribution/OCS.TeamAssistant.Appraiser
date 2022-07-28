using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class StartStorySelectionNotificationBuilder : INotificationBuilder<StartStorySelectionResult>
{
	public IEnumerable<INotificationMessage> Build(StartStorySelectionResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return NotificationMessage.Create(fromId, MessageId.EnterStoryName);
	}
}