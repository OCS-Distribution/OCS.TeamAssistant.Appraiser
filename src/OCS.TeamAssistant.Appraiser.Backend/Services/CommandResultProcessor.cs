using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTaskForEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Backend.Options;
using OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;

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

    public IEnumerable<Notification> Process(dynamic commandResult, long chatId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        if (commandResult is CreateAssessmentSessionResult)
            yield return Notification.Create("В ответ на это сообщение введите наименование сессии оценки.", chatId);

        if (commandResult is ActivateAssessmentSessionResult activateAssessmentSessionResult)
        {
            var linkForConnect = string.Format(
                _options.Value.LinkTemplate,
                activateAssessmentSessionResult.Id);

            yield return Notification.Create(
                $"Для подключения к \"{activateAssessmentSessionResult.Title}\" перейдите по ссылке {linkForConnect}",
                chatId);
        }

        if (commandResult is ConnectAppraiserResult connectAppraiserResult)
        {
            yield return Notification.Create(
                $"Успешное подключение к сессии оценки \"{connectAppraiserResult.AssessmentSessionTitle}\".",
                chatId);
            
            if (connectAppraiserResult.ChatId != chatId)
                yield return Notification.Create(
                    $"Участник {connectAppraiserResult.AppraiserName} подключен к сессии \"{connectAppraiserResult.AssessmentSessionTitle}\".",
                    connectAppraiserResult.ChatId);
        }

        if (commandResult is ShowAppraiserListResult showAppraiserListResult)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("К сессии подключены:");
            foreach (var appraiser in showAppraiserListResult.Appraisers)
                messageBuilder.AppendLine(appraiser);

            yield return Notification.Create(messageBuilder.ToString(), chatId);
        }


        if (commandResult is AddStoryResult addStoryResult)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Необходимо оценить задачу \"{addStoryResult.Title}\".");

            AddAssessments(messageBuilder);

            yield return Notification.Create(
                messageBuilder.ToString(),
                addStoryResult.AppraiserIds,
                (uId, sId, t) => SetStoryId(addStoryResult.AssessmentSessionId, uId, sId, t));
        }

        if (commandResult is EndEstimateResult showResultsResult)
            yield return Build(estimateEnded: true, showResultsResult.StoryTitle, showResultsResult.Items);

        if (commandResult is EstimateStoryResult estimateStoryResult)
            yield return Build(estimateStoryResult.EstimateEnded, estimateStoryResult.StoryTitle, estimateStoryResult.Items);

        if (commandResult is EndAssessmentSessionResult endAssessmentSessionResult)
            yield return Notification.Create(
                $"Сессия оценки \"{endAssessmentSessionResult.AssessmentSessionTitle}\" завершена.",
                chatId);

        if (commandResult is SendMessageResult sendMessageResult)
            yield return Notification.Create(sendMessageResult.Text, chatId);

        if (commandResult is ResetEstimateResult resetEstimateResult)
        {
            yield return Notification.Create(
                $"Запущена повторная оценка задачи \"{resetEstimateResult.StoryTitle}\".",
                resetEstimateResult.AppraiserIds);

            yield return Build(estimateEnded: false, resetEstimateResult.StoryTitle, resetEstimateResult.Items);
        }
    }

    private Notification Build(bool estimateEnded, string storyTitle, IReadOnlyCollection<EstimateItem> items)
    {
        if (string.IsNullOrWhiteSpace(storyTitle))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        
        var targetMessages = items.Select(i => (i.AppraiserId, i.StoryExternalId)).ToArray();
                
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(estimateEnded
            ? $"Завершена оценка задачи \"{storyTitle}\"."
            : $"Задача для оценки \"{storyTitle}\".");

        foreach (var item in items)
            messageBuilder.AppendLine($"{item.AppraiserName} {item.DisplayValue}");
        
        if (estimateEnded)
        {
            var values = items
                .Where(i => i.Value.HasValue)
                .Select(i => i.Value!.Value)
                .ToArray();
            
            if (values.Any())
                messageBuilder.AppendLine($"Среднее значение: {values.Sum() / (decimal)values.Length:.##} SP");
        }
        else
            AddAssessments(messageBuilder);

        return Notification.Edit(messageBuilder.ToString(), targetMessages);
    }
    
    private void AddAssessments(StringBuilder messageBuilder)
    {
        if (messageBuilder is null)
            throw new ArgumentNullException(nameof(messageBuilder));

        messageBuilder.Append("Выберите одну из оценок:");
        
        foreach (var assessment in AssessmentValueRules.GetAssessments)
            messageBuilder.Append($"/set{assessment} ");
        
        messageBuilder.Append("/set");
    }

    private async Task SetStoryId(
        Guid assessmentSessionId,
        long chatId,
        int messageId,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        await mediatr.Send(new AddTaskForEstimateCommand(
            assessmentSessionId,
            chatId,
            messageId), cancellationToken);
    }
}