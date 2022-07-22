using System.Text;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Models;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ResetEstimateNotificationBuilder : INotificationBuilder<ResetEstimateResult>
{
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
	private readonly IMessageBuilder _messageBuilder;

	public ResetEstimateNotificationBuilder(SummaryByStoryBuilder summaryByStoryBuilder, IMessageBuilder messageBuilder)
	{
		_summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
		_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
	}

	public IEnumerable<INotificationMessage> Build(ResetEstimateResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(_messageBuilder.Build(MessageId.EstimateRepeated, commandResult.StoryTitle));
		_summaryByStoryBuilder.AddAssessments(stringBuilder, _messageBuilder);

		yield return NotificationMessage.Create(commandResult.AppraiserIds, new(stringBuilder.ToString()));
		yield return _summaryByStoryBuilder.Build(estimateEnded: false, commandResult.StoryTitle, commandResult.Items);
	}
}