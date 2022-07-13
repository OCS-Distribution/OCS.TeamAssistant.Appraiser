using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTaskForEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Backend.Options;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class CommandResultProcessor
{
    private readonly IOptions<TelegramBotOptions> _options;
    private readonly IServiceProvider _serviceProvider;

    public CommandResultProcessor(IOptions<TelegramBotOptions> options, IServiceProvider serviceProvider)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
    
    public Notification Process(dynamic commandResult, long fromUserId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        if (commandResult is CreateAssessmentSessionResult)
            return Notification.Create("Введите наименование сессии оценки.", fromUserId);

        if (commandResult is ActivateAssessmentSessionResult activateAssessmentSessionResult)
        {
            var linkForConnect = string.Format(
                _options.Value.LinkTemplate,
                activateAssessmentSessionResult.Id);
            
            return Notification.Create(
                $"Для подключения к {activateAssessmentSessionResult.Title} перейдите по ссылке {linkForConnect}",
                fromUserId);
        }
        
        if (commandResult is ConnectAppraiserResult connectAppraiserResult)
            return Notification.Create(
                $"Успешное подключение к сессии оценки {connectAppraiserResult.AssessmentSessionTitle}.",
                fromUserId);
        
        if (commandResult is ShowAppraiserListResult showAppraiserListResult)
            return Notification.Create(
                $"К сессии подключены: {string.Join(", ", showAppraiserListResult.Appraisers)}",
                fromUserId);
        
        if (commandResult is AddStoryResult addStoryResult)
            return Notification.Create(
                $"Необходимо оценить задачу {addStoryResult.Title}. Начните вводить /set.",
                addStoryResult.AppraiserIds,
                (uId, sId, t) => SetStoryId(addStoryResult.AssessmentSessionId, uId, sId, t));
        
        if (commandResult is EndEstimateResult showResultsResult)
        {
            var targetMessages = showResultsResult.Items.Select(i => (i.AppraiserId, i.StoryExternalId)).ToArray();
                
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Завершена оценка задачи {showResultsResult.StoryTitle}.");
            
            foreach (var item in showResultsResult.Items)
                messageBuilder.AppendLine($"{item.AppraiserName}: {item.DisplayValue} SP");

            var values = showResultsResult.Items.Where(i => i.Value.HasValue).Select(i => i.Value!.Value).ToArray();
            if (values.Any())
                messageBuilder.AppendLine($"Среднее: {values.Sum() / values.Length} SP");

            return Notification.Edit(messageBuilder.ToString(), targetMessages);
        }
        
        if (commandResult is EstimateStoryResult estimateStoryResult)
        {
            var targetMessages = estimateStoryResult.Items.Select(i => (i.AppraiserId, i.StoryExternalId)).ToArray();
                
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Задача для оценки: {estimateStoryResult.StoryTitle}.");
            foreach (var item in estimateStoryResult.Items)
                messageBuilder.AppendLine($"{item.AppraiserName}: {(item.Exists ? "+" : string.Empty)}");

            return Notification.Edit(messageBuilder.ToString(), targetMessages);
        }

        if (commandResult is EndAssessmentSessionResult endAssessmentSessionResult)
            return Notification.Create(
                $"Сессия оценки {endAssessmentSessionResult.AssessmentSessionTitle} завершена.",
                fromUserId);
        
        if (commandResult is SendMessageResult sendMessageResult)
            return Notification.Create(sendMessageResult.Text, fromUserId);

        return Notification.Empty;
    }

    private async Task SetStoryId(
        Guid assessmentSessionId,
        long userId,
        int messageId,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        await mediatr.Send(new AddTaskForEstimateCommand(
            assessmentSessionId,
            userId,
            messageId), cancellationToken);
    }
}