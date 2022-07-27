using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Notifications.Services;

internal sealed class MessageBuilder : IMessageBuilder
{
    // TODO: move to file
    private readonly Dictionary<MessageId, string> _store = new()
    {
        [MessageId.NoRightsAddTaskToSession] = "Недостаточно прав для добавления задачи к сессии \"{0}\".",
        [MessageId.ShutdownCompletedWithError] = "Отключение завершено с ошибкой. Пользователь не подключен к сессии \"{0}\".",
        [MessageId.ModeratorCannotDisconnectedFromSession] = "Модератор не может быть отключен от сессии \"{0}\". Необходимо завершить сессию.",
        [MessageId.MissingTaskForEvaluate] = "Отсутствует задача для оценки. Дождитесь запуска процесса оценки.",
		[MessageId.NotValidState] = "Недопустимое состояние сессии \"{0}\" для применения операции.",

        [MessageId.SessionNotFoundForModerator] = "Не найдена сессия для модератора {0}.",
        [MessageId.SessionNotFoundForAppraiser] = "Не найдена сессия для участника {0}. Обратитесь к модератору.",
        [MessageId.AppraiserConnectWithError] = "{0} уже подключен к сессии {1}.",
        [MessageId.AppraiserConnectedToOtherSession] = "Подключение невозможно. Участник {0} подключен к другой сессии {1}.",
        [MessageId.EstimateRejected] = "Ваша оценка не принята. Оценка задачи \"{0}\" закончена.",

        [MessageId.ActiveSessionsFound] = "Найдены {0} активные сессии для участника {1}.",

        [MessageId.EnterSessionName] = "В ответ на это сообщение введите наименование сессии оценки.",
        [MessageId.EnterStoryName] = "В ответ на это сообщение введите наименование задачи для оценки.",
        [MessageId.ConnectToSession] = "Для подключения к \"{0}\" перейдите по ссылке и нажмите START {1}",
        [MessageId.ConnectedSuccess] = "Успешное подключение к сессии оценки \"{0}\".",
        [MessageId.AppraiserAdded] = "Участник {0} подключен к сессии \"{1}\".",
        [MessageId.DisconnectedFromSession] = "Успешное отключение от сессии \"{0}\".",
        [MessageId.AppraiserDisconnectedFromSession] = "Участник {0} отключен от сессии {1}.",
        [MessageId.SessionEnded] = "Сессия оценки \"{0}\" завершена.",
        [MessageId.EstimateRepeated] = "Запущена повторная оценка задачи \"{0}\".",
        [MessageId.EndEstimate] = "Завершена оценка задачи \"{0}\"",
        [MessageId.NeedEstimate] = "Необходимо оценить задачу \"{0}\".",
        [MessageId.TotalEstimate] = "Среднее значение: {0} SP",
        [MessageId.EnterEstimate] = "Выберите одну из оценок:",
        [MessageId.AppraiserList] = "К сессии подключены:",

        [MessageId.EnterCommandWithError] = "Команда введена неверно. Проверьте команду и попробуйте еще раз.",
        [MessageId.EnterEstimateWithError] = "Недопустимая оценка {0}. Список оценок: {1}",
        [MessageId.DisconnectCommandHelp] = "{0} - отключиться от сессии оценки",
        [MessageId.NewCommandHelp] = "{0} - создать сессию",
        [MessageId.UsersCommandHelp] = "{0} - список пользователей",
        [MessageId.AddCommandHelp] = "{0} - добавление задачи для оценки",
        [MessageId.SetCommandHelp] = "{0}{1} - задать оценку {1}",
        [MessageId.NoIdeaCommandHelp] = "{0} - без понятия",
        [MessageId.EndCommandHelp] = "{0} - завершение оценки",
        [MessageId.ResetCommandHelp] = "{0} - перезапустить оценку задачи",
        [MessageId.ExitCommandHelp] = "{0} - завершение сессии",
    };

    public string Build(MessageId messageId, params object[] values)
    {
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        if (_store.TryGetValue(messageId, out var message))
            return values.Any() ? string.Format(message, values) : message;

        return messageId.Value;
    }
}