using MediatR;
using MediatR.Registration;
using OCS.TeamAssistant.Appraiser.Application;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCommands(
		this IServiceCollection services,
		Action<MediatRServiceConfiguration>? configuration = null)
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		var config = new MediatRServiceConfiguration();
		configuration?.Invoke(config);
		ServiceRegistrar.AddRequiredServices(services, config);

		services
			.AddActivateAssessmentSessionCommand<ActivateAssessmentSessionCommand>()
			.AddAddStoryCommand<AddStoryCommand>()
			.AddConnectAppraiserCommand<ConnectAppraiserCommand>()
			.AddCreateAssessmentSessionCommand<CreateAssessmentSessionCommand>()
			.AddDisconnectAppraiserCommand<DisconnectAppraiserCommand>()
			.AddEndAssessmentSessionCommand<EndAssessmentSessionCommand>()
			.AddEndEstimateCommand<EndEstimateCommand>()
			.AddEstimateStoryCommand<EstimateStoryCommand>()
			.AddResetEstimateCommand<ResetEstimateCommand>()
			.AddSendMessageCommand<SendMessageCommand>()
			.AddShowAppraiserListQuery<ShowAppraiserListQuery>()
			.AddStartStorySelectionCommand<StartStorySelectionCommand>();

		return services;
	}
}