using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Models;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class EndAssessmentSessionNotificationBuilder : INotificationBuilder<EndAssessmentSessionResult>
{
	public IEnumerable<INotificationMessage> Build(EndAssessmentSessionResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var targets = commandResult.AppraiserIds.Append(fromId).Distinct().ToArray();

		yield return NotificationMessage.Create(
			targets,
			MessageId.SessionEnded,
			commandResult.AssessmentSessionTitle);
	}
}