using Microsoft.Extensions.DependencyInjection;
using OCS.TeamAssistant.Appraiser.Application;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Connect;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessment;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Disconnect;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessment;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;
using OCS.TeamAssistant.Appraiser.Notifications.Builders;
using OCS.TeamAssistant.Appraiser.Notifications.Commands;
using OCS.TeamAssistant.Appraiser.Notifications.Services;

namespace OCS.TeamAssistant.Appraiser.Notifications;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddNotifications(
		this IServiceCollection services,
		string linkTemplate,
		string setCommand,
		string noIdeaCommand)
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));
		if (String.IsNullOrWhiteSpace(linkTemplate))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkTemplate));
		if (String.IsNullOrWhiteSpace(setCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(setCommand));
		if (String.IsNullOrWhiteSpace(noIdeaCommand))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(noIdeaCommand));

		services
			.AddSingleton<IMessageBuilder, MessageBuilder>()
			.AddSingleton(sp => ActivatorUtilities.CreateInstance<SummaryByStoryBuilder>(sp, setCommand, noIdeaCommand))

			.AddSingleton<INotificationBuilder<ActivateAssessmentResult>>(
				sp => ActivatorUtilities.CreateInstance<ActivateAssessmentNotificationBuilder>(
					sp,
					linkTemplate))
			.AddSingleton<INotificationBuilder<AddStoryResult>, AddStoryNotificationBuilder>()
			.AddSingleton<INotificationBuilder<ConnectResult>, ConnectNotificationBuilder>()
			.AddSingleton<INotificationBuilder<CreateAssessmentResult>, CreateAssessmentNotificationBuilder>()
			.AddSingleton<INotificationBuilder<DisconnectResult>, DisconnectNotificationBuilder>()
			.AddSingleton<INotificationBuilder<EndAssessmentResult>, EndAssessmentNotificationBuilder>()
			.AddSingleton<INotificationBuilder<EndEstimateResult>, EndEstimateNotificationBuilder>()
			.AddSingleton<INotificationBuilder<EstimateStoryResult>, EstimateStoryNotificationBuilder>()
			.AddSingleton<INotificationBuilder<ResetEstimateResult>, ResetEstimateNotificationBuilder>()
			.AddSingleton<INotificationBuilder<SendMessageResult>, SendMessageNotificationBuilder>()
			.AddSingleton<INotificationBuilder<ShowParticipantsResult>, ShowParticipantsNotificationBuilder>()
			.AddSingleton<INotificationBuilder<StartStorySelectionResult>, StartStorySelectionNotificationBuilder>()

			.AddAddStoryForEstimateCommand<AddStoryForEstimateCommand>();

		return services;
	}
}