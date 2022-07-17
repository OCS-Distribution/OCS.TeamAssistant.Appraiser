using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTaskForEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.DisconnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.Restart;
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
                $"Для подключения к \"{activateAssessmentSessionResult.Title}\" перейдите по ссылке и нажмите start {linkForConnect}",
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

        if (commandResult is DisconnectAppraiserResult disconnectAppraiserResult)
        {
            yield return Notification.Create(
                $"Успешное отключение от сессии \"{disconnectAppraiserResult.AssessmentSessionTitle}\".",
                chatId);

            if (disconnectAppraiserResult.ChatId != chatId)
                yield return Notification.Create(
                    $"Участник {disconnectAppraiserResult.AppraiserName} отключен от сессии {disconnectAppraiserResult.AssessmentSessionTitle}.",
                    disconnectAppraiserResult.ChatId);
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

            AddEstimateItems(messageBuilder, addStoryResult.Items, estimateEnded: false);
            
            AddAssessments(messageBuilder);

            yield return Notification.Create(
                messageBuilder.ToString(),
                addStoryResult.AppraiserIds,
                (cId, uId, mId, t) => SetStoryId(addStoryResult.AssessmentSessionId, cId, uId, mId, t));
        }

        if (commandResult is EndEstimateResult showResultsResult)
        {
            yield return Build(estimateEnded: false, showResultsResult.StoryTitle, showResultsResult.Items);
            yield return Build(estimateEnded: true, showResultsResult.StoryTitle, showResultsResult.Items);
        }

        if (commandResult is EstimateStoryResult estimateStoryResult)
        {
            yield return Build(estimateEnded: false, estimateStoryResult.StoryTitle, estimateStoryResult.Items);
            if (estimateStoryResult.EstimateEnded)
                yield return Build(estimateEnded: true, estimateStoryResult.StoryTitle, estimateStoryResult.Items);
        }

        if (commandResult is EndAssessmentSessionResult endAssessmentSessionResult)
        {
            var targets = endAssessmentSessionResult.AppraiserIds.Append(chatId).Distinct().ToArray();
            
            yield return Notification.Create(
                $"Сессия оценки \"{endAssessmentSessionResult.AssessmentSessionTitle}\" завершена.",
                targets);
        }
        
        if (commandResult is SendMessageResult sendMessageResult)
            yield return Notification.Create(sendMessageResult.Text, chatId);

        if (commandResult is ResetEstimateResult resetEstimateResult)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Запущена повторная оценка задачи \"{resetEstimateResult.StoryTitle}\".");
            AddAssessments(messageBuilder);
            
            yield return Notification.Create(messageBuilder.ToString(), resetEstimateResult.AppraiserIds);
            yield return Build(estimateEnded: false, resetEstimateResult.StoryTitle, resetEstimateResult.Items);
        }

        if (commandResult is RestartResult)
            yield return Notification.Create("Память бота очищена.", chatId);
    }

    private Notification Build(bool estimateEnded, string storyTitle, IReadOnlyCollection<EstimateItem> items)
    {
        if (string.IsNullOrWhiteSpace(storyTitle))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(estimateEnded
            ? $"Завершена оценка задачи \"{storyTitle}\"."
            : $"Необходимо оценить задачу \"{storyTitle}\".");

        AddEstimateItems(messageBuilder, items, estimateEnded);
        
        if (estimateEnded)
        {
            var values = items
                .Where(i => AssessmentValueRules.GetAssessments.Contains(i.Value))
                .Select(i => (int)i.Value)
                .ToArray();
            
            if (values.Any())
                messageBuilder.AppendLine($"Среднее значение: {values.Sum() / (decimal)values.Length:.##} SP");
        }
        else
            AddAssessments(messageBuilder);

        return estimateEnded
            ? Notification.Create(messageBuilder.ToString(), items.Select(i => i.AppraiserId).ToArray())
            : Notification.Edit(messageBuilder.ToString(), items.Select(i => (i.AppraiserId, i.StoryExternalId)).ToArray());
    }

    private void AddEstimateItems(
        StringBuilder messageBuilder,
        IReadOnlyCollection<EstimateItem> items,
        bool estimateEnded)
    {
        if (messageBuilder is null)
            throw new ArgumentNullException(nameof(messageBuilder));
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        
        foreach (var item in items)
            messageBuilder.AppendLine($"{item.AppraiserName} {DisplayValue(item.Value, estimateEnded)}");
    }
    
    private string DisplayValue(AssessmentValue value, bool estimateEnded)
    {
        if (estimateEnded)
            return value switch
            {
                AssessmentValue.Unknown => "?",
                AssessmentValue.None => "-",
                _ => ((int)value).ToString()
            };

        return value == AssessmentValue.None ? string.Empty : "+";
    }
    
    private void AddAssessments(StringBuilder messageBuilder)
    {
        if (messageBuilder is null)
            throw new ArgumentNullException(nameof(messageBuilder));

        messageBuilder.Append("Выберите одну из оценок:");
        
        foreach (var assessment in AssessmentValueRules.GetAssessments)
            messageBuilder.Append($" /{assessment.ToString().ToLower()}");
        
        messageBuilder.Append(" /noidea");
    }

    private async Task SetStoryId(
        Guid assessmentSessionId,
        long chatId,
        string userName,
        int messageId,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        await mediatr.Send(new AddTaskForEstimateCommand(
            assessmentSessionId,
            chatId,
            userName,
            messageId), cancellationToken);
    }
}