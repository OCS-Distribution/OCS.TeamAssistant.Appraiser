using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;
using OCS.TeamAssistant.Appraiser.Application.QueryHandlers.ShowAppraiserList;

namespace OCS.TeamAssistant.Appraiser.Application;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddActivateAssessmentSessionCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IActivateAssessmentSessionCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, ActivateAssessmentSessionResult>, ActivateAssessmentSessionCommandHandler>();

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
		where TCommand : class, IConnectAppraiserCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, ConnectAppraiserResult>, ConnectAppraiserCommandHandler>();

		return services;
	}

	public static IServiceCollection AddCreateAssessmentSessionCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, ICreateAssessmentSessionCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, CreateAssessmentSessionResult>, CreateAssessmentSessionCommandHandler>();

		return services;
	}

	public static IServiceCollection AddDisconnectAppraiserCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IDisconnectAppraiserCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, DisconnectAppraiserResult>, DisconnectAppraiserCommandHandler>();

		return services;
	}

	public static IServiceCollection AddEndAssessmentSessionCommand<TCommand>(this IServiceCollection services)
		where TCommand : class, IEndAssessmentSessionCommand
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TCommand, EndAssessmentSessionResult>, EndAssessmentSessionCommandHandler>();

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
		where TQuery : class, IShowAppraiserListQuery
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));

		services
			.AddScoped<IRequestHandler<TQuery, ShowAppraiserListResult>, ShowAppraiserListCommandHandler>();

		return services;
	}
}