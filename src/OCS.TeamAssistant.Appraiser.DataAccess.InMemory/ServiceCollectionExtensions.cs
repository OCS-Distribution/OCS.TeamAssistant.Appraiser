using Microsoft.Extensions.DependencyInjection;
using OCS.TeamAssistant.Appraiser.Application.Contracts;

namespace OCS.TeamAssistant.Appraiser.DataAccess.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryDataAccess(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<IAssessmentSessionRepository, AssessmentSessionInMemoryRepository>();

        return services;
    }
}