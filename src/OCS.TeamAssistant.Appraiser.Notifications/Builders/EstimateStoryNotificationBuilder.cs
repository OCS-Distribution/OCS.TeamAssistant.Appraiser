using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class EstimateStoryNotificationBuilder : INotificationBuilder<EstimateStoryResult>
{
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;

	public EstimateStoryNotificationBuilder(SummaryByStoryBuilder summaryByStoryBuilder)
	{
		_summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
	}

	public IEnumerable<INotificationMessage> Build(EstimateStoryResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		yield return _summaryByStoryBuilder.Build(estimateEnded: false, commandResult.StoryTitle, commandResult.Items);

		if (commandResult.EstimateEnded)
			yield return _summaryByStoryBuilder.Build(estimateEnded: true, commandResult.StoryTitle, commandResult.Items);
	}
}