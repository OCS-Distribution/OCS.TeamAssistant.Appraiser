using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessment;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class CreateAssessmentNotificationBuilder : INotificationBuilder<CreateAssessmentResult>
{
	public IEnumerable<INotificationMessage> Build(CreateAssessmentResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return NotificationMessage.Create(fromId, MessageId.EnterSessionName);
	}
}