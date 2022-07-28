using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessment;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class EndAssessmentNotificationBuilder : INotificationBuilder<EndAssessmentResult>
{
	public IEnumerable<INotificationMessage> Build(EndAssessmentResult commandResult, long fromId)
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