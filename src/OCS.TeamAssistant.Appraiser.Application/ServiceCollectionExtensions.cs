using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Connect;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessment;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Disconnect;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessment;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;
using OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;

namespace OCS.TeamAssistant.Appraiser.Application;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddActivateAssessmentSessionCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IActivateAssessmentCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, ActivateAssessmentResult>, ActivateAssessmentCommandHandler>();

		return services;
	}

	public static IServiceCollection AddAddStoryCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IAddStoryCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, AddStoryResult>, AddStoryCommandHandler>();

		return services;
	}

	public static IServiceCollection AddAddStoryForEstimateCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IAddStoryForEstimateCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, AddStoryForEstimateResult>, AddStoryForEstimateCommandHandler>();

		return services;
	}

	public static IServiceCollection AddConnectAppraiserCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IConnectCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, ConnectResult>, ConnectCommandHandler>();

		return services;
	}

	public static IServiceCollection AddCreateAssessmentSessionCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, ICreateAssessmentCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, CreateAssessmentResult>, CreateAssessmentCommandHandler>();

		return services;
	}

	public static IServiceCollection AddDisconnectAppraiserCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IDisconnectCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, DisconnectResult>, DisconnectCommandHandler>();

		return services;
	}

	public static IServiceCollection AddEndAssessmentSessionCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IEndAssessmentCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, EndAssessmentResult>, EndAssessmentCommandHandler>();

		return services;
	}

	public static IServiceCollection AddEndEstimateCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IEndEstimateCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, EndEstimateResult>, EndEstimateCommandHandler>();

		return services;
	}

	public static IServiceCollection AddEstimateStoryCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IEstimateStoryCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, EstimateStoryResult>, EstimateStoryCommandHandler>();

		return services;
	}

	public static IServiceCollection AddResetEstimateCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IResetEstimateCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, ResetEstimateResult>, ResetEstimateCommandHandler>();

		return services;
	}

	public static IServiceCollection AddSendMessageCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, ISendMessageCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, SendMessageResult>, SendMessageCommandHandler>();

		return services;
	}

	public static IServiceCollection AddStartStorySelectionCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IStartStorySelectionCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, StartStorySelectionResult>, StartStorySelectionCommandHandler>();

		return services;
	}

	public static IServiceCollection AddShowAppraiserListQuery<TQuery>(this IServiceCollection services)
		where TQuery : class, IShowParticipantsQuery
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TQuery, ShowParticipantsResult>, ShowParticipantsCommandHandler>();

		return services;
	}
}