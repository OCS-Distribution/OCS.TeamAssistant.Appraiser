using System.Text;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Models;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ShowAppraiserListNotificationBuilder : INotificationBuilder<ShowAppraiserListResult>
{
	private readonly IMessageBuilder _messageBuilder;

	public ShowAppraiserListNotificationBuilder(IMessageBuilder messageBuilder)
	{
		_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
	}

	public IEnumerable<INotificationMessage> Build(ShowAppraiserListResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var messageBuilder = new StringBuilder();
		messageBuilder.AppendLine(_messageBuilder.Build(MessageId.AppraiserList));
		foreach (var appraiser in commandResult.Appraisers)
			messageBuilder.AppendLine(appraiser);

		yield return NotificationMessage.Create(fromId, new(messageBuilder.ToString()));
	}
}