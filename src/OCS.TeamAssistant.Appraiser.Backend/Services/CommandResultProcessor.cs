using System.Text;
using Microsoft.Extensions.Options;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;
using OCS.TeamAssistant.Appraiser.Backend.Options;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class CommandResultProcessor
{
    private readonly IOptions<TelegramBotOptions> _options;

    public CommandResultProcessor(IOptions<TelegramBotOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    public Notification Process(dynamic commandResult, long fromUserId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        if (commandResult is CreateAssessmentSessionResult)
            return new Notification("Введите наименование сессии оценки.", new []{ fromUserId });
        
        if (commandResult is ActivateAssessmentSessionResult activateAssessmentSessionResult)
            return new Notification(
                string.Format(_options.Value.LinkTemplate, activateAssessmentSessionResult.AssessmentSessionId),
                new []{ fromUserId });
        
        if (commandResult is ConnectAppraiserResult connectAppraiserResult)
            return new Notification(
                $"Успешное подключение к сессии оценки {connectAppraiserResult.AssessmentSessionTitle}.",
                new []{ fromUserId });
        
        if (commandResult is ShowAppraiserListResult showAppraiserListResult)
            return new Notification(
                $"К сессии подключены: {string.Join(", ", showAppraiserListResult.Appraisers)}",
                new []{ fromUserId });
        
        if (commandResult is AddStoryResult addStoryResult)
            return new Notification($"Необходимо оценить задачу {addStoryResult.Title}.", addStoryResult.AppraiserIds);
        
        if (commandResult is EndEstimateResult showResultsResult)
        {
            var userIds = showResultsResult.Items.Select(i => i.AppraiserId).ToArray();
            
            var messageBuilder = new StringBuilder();
            foreach (var item in showResultsResult.Items)
                messageBuilder.AppendLine($"{item.AppraiserName}: {item.Value} SP");

            return new Notification(messageBuilder.ToString(), userIds);
        }

        if (commandResult is EndAssessmentSessionResult endAssessmentSessionResult)
            return new Notification(
                $"Сессия оценки {endAssessmentSessionResult.AssessmentSessionTitle} завершена.",
                new []{ fromUserId });
        
        if (commandResult is SendMessageResult sendMessageResult)
            return new Notification(sendMessageResult.Text, new []{ fromUserId });

        return Notification.Empty;
    }
}