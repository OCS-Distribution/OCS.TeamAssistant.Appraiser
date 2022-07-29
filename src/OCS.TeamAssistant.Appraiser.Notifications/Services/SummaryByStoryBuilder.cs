using System.Text;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Notifications.Services;

internal sealed class SummaryByStoryBuilder
{
	private readonly IMessageBuilder _messageBuilder;
	private readonly string _setCommand;
	private readonly string _noIdeaCommand;

	public SummaryByStoryBuilder(IMessageBuilder messageBuilder, string setCommand, string noIdeaCommand)
	{
		if (String.IsNullOrWhiteSpace(setCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(setCommand));
		if (String.IsNullOrWhiteSpace(noIdeaCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(noIdeaCommand));

		_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
		_setCommand = setCommand;
		_noIdeaCommand = noIdeaCommand;
	}

	public INotificationMessage Build(
		bool estimateEnded,
		string storyTitle,
		decimal? total,
		IReadOnlyCollection<IEstimateItem> items)
	{
		if (string.IsNullOrWhiteSpace(storyTitle))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));
		if (items is null)
			throw new ArgumentNullException(nameof(items));

		var builder = new StringBuilder();
		var headerMessageId = estimateEnded ? MessageId.EndEstimate : MessageId.NeedEstimate;
		builder.AppendLine(_messageBuilder.Build(headerMessageId, storyTitle));

		foreach (var item in items)
		{
			var value = estimateEnded ? item.Value.DisplayValue() : item.Value.DisplayHasValue();
			builder.AppendLine($"{item.AppraiserName} {value}");
		}

		if (estimateEnded)
			builder.AppendLine(_messageBuilder.Build(MessageId.TotalEstimate, total.DisplayValue()));
		else
			AddAssessments(builder, _messageBuilder);

		var messageText = new MessageId(builder.ToString());
		return estimateEnded
			? NotificationMessage.Create(items.Select(i => i.AppraiserId).ToArray(), messageText)
			: NotificationMessage.Edit(items.Select(i => (i.AppraiserId, i.StoryExternalId)).ToArray(), messageText);
	}

	public void AddAssessments(StringBuilder stringBuilder, IMessageBuilder messageBuilder)
	{
		if (stringBuilder is null)
			throw new ArgumentNullException(nameof(stringBuilder));
		if (messageBuilder is null)
			throw new ArgumentNullException(nameof(messageBuilder));

		stringBuilder.Append(messageBuilder.Build(MessageId.EnterEstimate));

		foreach (var assessment in AssessmentValueRules.GetAssessments)
			stringBuilder.Append($" {_setCommand}{(int)assessment}");

		stringBuilder.Append($" {_noIdeaCommand}");
	}
}