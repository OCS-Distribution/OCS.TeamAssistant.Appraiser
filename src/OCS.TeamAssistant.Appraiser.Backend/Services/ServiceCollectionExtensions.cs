using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Model;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServices(this IServiceCollection services, string accessToken)
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));
		if (String.IsNullOrWhiteSpace(accessToken))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));

		services
			.AddSingleton<CommandFactory>()
			.AddHostedService(sp => ActivatorUtilities.CreateInstance<TelegramBotListener>(sp, accessToken))
			.AddSingleton<TelegramBotMessageHandler>()
			.AddScoped<IAssessmentSessionsService, AssessmentSessionsService>()
			.AddScoped<IMessagesService, MessagesService>();

		return services;
	}
}