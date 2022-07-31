using System.Text;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Extensions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using OCS.TeamAssistant.Appraiser.Notifications.Commands;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class AddStoryNotificationBuilder : INotificationBuilder<AddStoryResult>
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IMessageBuilder _messageBuilder;
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;

	public AddStoryNotificationBuilder(
		IServiceProvider serviceProvider,
		IMessageBuilder messageBuilder,
		SummaryByStoryBuilder summaryByStoryBuilder)
	{
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
		_summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
	}

	public IEnumerable<INotificationMessage> Build(AddStoryResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var appraiserIds = commandResult.Items.Select(i => i.AppraiserId).ToArray();
		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(_messageBuilder.Build(MessageId.NeedEstimate, commandResult.Title));

		foreach (var item in commandResult.Items)
			stringBuilder.AppendLine($"{item.AppraiserName} {item.Value.DisplayHasValue()}");

		_summaryByStoryBuilder.AddAssessments(stringBuilder, _messageBuilder);

		yield return NotificationMessage
			.Create(appraiserIds, new(stringBuilder.ToString()))
			.AddHandler((cId, uId, mId, t) => AddStoryForEstimate(commandResult.AssessmentSessionId, cId, uId, mId, t));
	}

	private async Task AddStoryForEstimate(
		Guid assessmentSessionId,
		long chatId,
		string userName,
		int messageId,
		CancellationToken cancellationToken)
	{
		if (String.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

		using var scope = _serviceProvider.CreateScope();
		var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

		await mediatr.Send(new AddStoryForEstimateCommand(
			assessmentSessionId,
			chatId,
			userName,
			messageId), cancellationToken);
	}
}