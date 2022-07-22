using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Models;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ActivateAssessmentSessionNotificationBuilder
	: INotificationBuilder<ActivateAssessmentSessionResult>
{
	private readonly string _linkTemplate;

	public ActivateAssessmentSessionNotificationBuilder(string linkTemplate)
	{
		if (String.IsNullOrWhiteSpace(linkTemplate))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkTemplate));

		_linkTemplate = linkTemplate;
	}

	public IEnumerable<INotificationMessage> Build(ActivateAssessmentSessionResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var linkForConnect = string.Format(_linkTemplate, commandResult.Id);

		yield return NotificationMessage.Create(fromId, MessageId.ConnectToSession, commandResult.Title, linkForConnect);
	}
}