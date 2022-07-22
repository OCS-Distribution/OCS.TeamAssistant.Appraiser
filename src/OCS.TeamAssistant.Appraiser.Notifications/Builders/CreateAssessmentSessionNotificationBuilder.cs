using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Models;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class CreateAssessmentSessionNotificationBuilder : INotificationBuilder<CreateAssessmentSessionResult>
{
	public IEnumerable<INotificationMessage> Build(CreateAssessmentSessionResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return NotificationMessage.Create(fromId, MessageId.EnterSessionName);
	}
}