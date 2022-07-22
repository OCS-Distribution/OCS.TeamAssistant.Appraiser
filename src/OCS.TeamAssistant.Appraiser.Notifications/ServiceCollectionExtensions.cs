using Microsoft.Extensions.DependencyInjection;
using OCS.TeamAssistant.Appraiser.Application;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowAppraiserList;
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

			.AddSingleton<INotificationBuilder<ActivateAssessmentSessionResult>>(
				sp => ActivatorUtilities.CreateInstance<ActivateAssessmentSessionNotificationBuilder>(
					sp,
					linkTemplate))
			.AddSingleton<INotificationBuilder<AddStoryResult>, AddStoryNotificationBuilder>()
			.AddSingleton<INotificationBuilder<ConnectAppraiserResult>, ConnectAppraiserNotificationBuilder>()
			.AddSingleton<INotificationBuilder<CreateAssessmentSessionResult>, CreateAssessmentSessionNotificationBuilder>()
			.AddSingleton<INotificationBuilder<DisconnectAppraiserResult>, DisconnectAppraiserNotificationBuilder>()
			.AddSingleton<INotificationBuilder<EndAssessmentSessionResult>, EndAssessmentSessionNotificationBuilder>()
			.AddSingleton<INotificationBuilder<EndEstimateResult>, EndEstimateNotificationBuilder>()
			.AddSingleton<INotificationBuilder<EstimateStoryResult>, EstimateStoryNotificationBuilder>()
			.AddSingleton<INotificationBuilder<ResetEstimateResult>, ResetEstimateNotificationBuilder>()
			.AddSingleton<INotificationBuilder<SendMessageResult>, SendMessageNotificationBuilder>()
			.AddSingleton<INotificationBuilder<ShowAppraiserListResult>, ShowAppraiserListNotificationBuilder>()
			.AddSingleton<INotificationBuilder<StartStorySelectionResult>, StartStorySelectionNotificationBuilder>()

			.AddAddStoryForEstimateCommand<AddStoryForEstimateCommand>();

		return services;
	}
}