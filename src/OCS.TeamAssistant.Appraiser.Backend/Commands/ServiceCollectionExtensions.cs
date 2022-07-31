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
			.AddActivateAssessmentSessionCommand<ActivateAssessmentCommand>()
			.AddAddStoryCommand<AddStoryCommand>()
			.AddConnectAppraiserCommand<ConnectCommand>()
			.AddCreateAssessmentSessionCommand<CreateAssessmentCommand>()
			.AddDisconnectAppraiserCommand<DisconnectCommand>()
			.AddEndAssessmentSessionCommand<EndAssessmentCommand>()
			.AddEndEstimateCommand<EndEstimateCommand>()
			.AddEstimateStoryCommand<EstimateStoryCommand>()
			.AddResetEstimateCommand<ResetEstimateCommand>()
			.AddSendMessageCommand<SendMessageCommand>()
			.AddShowAppraiserListQuery<ShowParticipantsQuery>()
			.AddStartStorySelectionCommand<StartStorySelectionCommand>()
			.AddQueries();

		return services;
	}
}