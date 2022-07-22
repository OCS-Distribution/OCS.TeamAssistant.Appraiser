using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class EndEstimateNotificationBuilder : INotificationBuilder<EndEstimateResult>
{
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;

	public EndEstimateNotificationBuilder(SummaryByStoryBuilder summaryByStoryBuilder)
	{
		_summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
	}

	public IEnumerable<INotificationMessage> Build(EndEstimateResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return _summaryByStoryBuilder.Build(estimateEnded: false, commandResult.StoryTitle, commandResult.Items);
		yield return _summaryByStoryBuilder.Build(estimateEnded: true, commandResult.StoryTitle, commandResult.Items);
	}
}