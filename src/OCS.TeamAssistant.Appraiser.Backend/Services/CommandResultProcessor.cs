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
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class CommandResultProcessor
{
    private readonly IOptions<TelegramBotOptions> _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBuilder _messageBuilder;

    public CommandResultProcessor(
        IOptions<TelegramBotOptions> options,
        IServiceProvider serviceProvider,
        IMessageBuilder messageBuilder)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public IEnumerable<Notification> Process(dynamic commandResult, long chatId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        if (commandResult is CreateAssessmentSessionResult)
            yield return Notification.Create(chatId, MessageId.EnterSessionName);

        if (commandResult is ActivateAssessmentSessionResult activateAssessmentSessionResult)
        {
            var linkForConnect = string.Format(
                _options.Value.LinkTemplate,
                activateAssessmentSessionResult.Id);

            yield return Notification.Create(
                chatId, MessageId.ConnectToSession, activateAssessmentSessionResult.Title, linkForConnect);
        }

        if (commandResult is ConnectAppraiserResult connectAppraiserResult)
        {
            yield return Notification.Create(
                chatId,
                MessageId.ConnectedSuccess,
                connectAppraiserResult.AssessmentSessionTitle);
            
            if (connectAppraiserResult.ChatId != chatId)
                yield return Notification.Create(
                    connectAppraiserResult.ChatId,
                    MessageId.AppraiserAdded,
                    connectAppraiserResult.AppraiserName,
                    connectAppraiserResult.AssessmentSessionTitle);
        }

        if (commandResult is DisconnectAppraiserResult disconnectAppraiserResult)
        {
            yield return Notification.Create(
                chatId,
                MessageId.DisconnectedFromSession,
                disconnectAppraiserResult.AssessmentSessionTitle);

            if (disconnectAppraiserResult.ChatId != chatId)
                yield return Notification.Create(
                    disconnectAppraiserResult.ChatId,
                    MessageId.AppraiserDisconnectedFromSession,
                    disconnectAppraiserResult.AppraiserName,
                    disconnectAppraiserResult.AssessmentSessionTitle);
        }

        if (commandResult is ShowAppraiserListResult showAppraiserListResult)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(_messageBuilder.Build(MessageId.AppraiserList));
            foreach (var appraiser in showAppraiserListResult.Appraisers)
                messageBuilder.AppendLine(appraiser);

            yield return Notification.Create(chatId, new MessageId(messageBuilder.ToString()));
        }

        if (commandResult is AddStoryResult addStoryResult)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(_messageBuilder.Build(MessageId.NeedEstimate, addStoryResult.Title));

            AddEstimateItems(messageBuilder, addStoryResult.Items, estimateEnded: false);
            
            AddAssessments(messageBuilder);

            yield return Notification
                .Create(addStoryResult.AppraiserIds, new MessageId(messageBuilder.ToString()))
                .AddHandler((cId, uId, mId, t) => SetStoryId(addStoryResult.AssessmentSessionId, cId, uId, mId, t));
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
                targets,
                MessageId.SessionEnded,
                endAssessmentSessionResult.AssessmentSessionTitle);
        }
        
        if (commandResult is SendMessageResult sendMessageResult)
            yield return Notification.Create(chatId, new MessageId(sendMessageResult.Text));

        if (commandResult is ResetEstimateResult resetEstimateResult)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(_messageBuilder.Build(MessageId.EstimateRepeated, resetEstimateResult.StoryTitle));
            AddAssessments(messageBuilder);
            
            yield return Notification.Create(resetEstimateResult.AppraiserIds, new MessageId(messageBuilder.ToString()));
            yield return Build(estimateEnded: false, resetEstimateResult.StoryTitle, resetEstimateResult.Items);
        }

        if (commandResult is RestartResult)
            yield return Notification.Create(chatId, MessageId.MemoryCleared);
    }

    private Notification Build(bool estimateEnded, string storyTitle, IReadOnlyCollection<EstimateItem> items)
    {
        if (string.IsNullOrWhiteSpace(storyTitle))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var messageBuilder = new StringBuilder();
        var headerMessageId = estimateEnded ? MessageId.EndEstimate : MessageId.NeedEstimate;
        messageBuilder.AppendLine(_messageBuilder.Build(headerMessageId, storyTitle));

        AddEstimateItems(messageBuilder, items, estimateEnded);
        
        if (estimateEnded)
        {
            var values = items
                .Where(i => AssessmentValueRules.GetAssessments.Contains(i.Value))
                .Select(i => (int)i.Value)
                .ToArray();

            var total = values.Sum() / (decimal)values.Length;
            if (values.Any())
                messageBuilder.AppendLine(_messageBuilder.Build(MessageId.TotalEstimate, total));
        }
        else
            AddAssessments(messageBuilder);

        var messageText = new MessageId(messageBuilder.ToString());
        return estimateEnded
            ? Notification.Create(items.Select(i => i.AppraiserId).ToArray(), messageText)
            : Notification.Edit(items.Select(i => (i.AppraiserId, i.StoryExternalId)).ToArray(), messageText);
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

        messageBuilder.Append(_messageBuilder.Build(MessageId.EnterEstimate));
        
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