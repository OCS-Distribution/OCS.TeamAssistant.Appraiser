using System.Text;
using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimates;
using OCS.TeamAssistant.Appraiser.Domain;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class CommandFactory
{
    const int ParameterIndex = 1;
    private readonly Dictionary<string, (Func<string, long, string, IBaseRequest?> Action, string Example)> _commandList;
    private readonly IServiceProvider _serviceProvider;
    private readonly HashSet<string> _targets = new(new[] { "1", "2", "3", "5", "8", "13", "21" });

    public CommandFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _commandList = new()
        {
            ["/start"] = (CreateConnectAppraiserCommand, string.Empty),
            ["/new"] = ((c, uId, uName) => new CreateAssessmentSessionCommand(uId, uName), "/new - создать сессию"),
            ["/users"] = ((c, uId, uName) => new ShowAppraiserListCommand(uId), "/users - список пользователей"),
            ["/add"] = ((c, uId, uName) => CreateAddStoryCommand(c, uId), "/add {task} - добавление задачи для оценки"),
            ["/set"] = ((c, uId, uName) => CreateEstimateStoryCommand(c, uId), "/set{3} - задать оценку"),
            ["/end"] = ((c, uId, uName) => new EndEstimateCommand(uId), "/end - завершение оценки"),
            ["/exit"] = ((c, uId, uName) => new EndAssessmentSessionCommand(uId), "/exit - завершение сессии"),
            ["/help"] = ((c, uId, uName) => CreateHelpCommand(), "/help - помощь")
        };
    }

    public async Task<IBaseRequest> Create(
        string commandText,
        long userId,
        string userName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        
        IBaseRequest? command = null;

        var commandKey = _commandList.Keys.FirstOrDefault(commandText.StartsWith);
        if (commandKey is not null)
            command = _commandList[commandKey].Action(commandText, userId, userName);

        command ??= await FindDraftSession(commandText, userId, cancellationToken);
        command ??= CreateErrorHandleCommand();
        
        return command;
    }

    private AddStoryCommand? CreateAddStoryCommand(string commandText, long userId)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        
        var commands = commandText.Split(' ');
        var title = commands.Length > ParameterIndex
            ? commands[ParameterIndex]
            : string.Empty;

        return !string.IsNullOrWhiteSpace(title)
            ? new AddStoryCommand(userId, title)
            : null;
    }

    public IBaseRequest CreateErrorHandleCommand()
    {
        return new SendMessageCommand("Команда введена неверно. Проверьте команду и попробуйте еще раз.");
    }

    private SendMessageCommand CreateHelpCommand()
    {
        var messageBuilder = new StringBuilder();

        foreach (var command in _commandList.Values)
            if (!string.IsNullOrWhiteSpace(command.Example))
                messageBuilder.AppendLine(command.Example);

        return new SendMessageCommand(messageBuilder.ToString());
    }

    private IBaseRequest CreateEstimateStoryCommand(string commandText, long userId)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));

        const int estimateIndex = 4;

        if (commandText.Length <= estimateIndex)
            return new EstimateStoryCommand(userId, Value: null);

        var parameter = commandText.Substring(estimateIndex, commandText.Length - estimateIndex);

        return _targets.Contains(parameter)
            ? new EstimateStoryCommand(userId, int.Parse(parameter))
            : new SendMessageCommand($"Недопустимая оценка {parameter}. Список оценок: {string.Join(", ", _targets)}");
    }

    private ConnectAppraiserCommand? CreateConnectAppraiserCommand(string commandText, long userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        
        var commands = commandText.Split(' ');
        var assessmentSessionId = commands.Length > ParameterIndex
            ? commands[ParameterIndex]
            : string.Empty;

        return !string.IsNullOrWhiteSpace(userName) && Guid.TryParse(assessmentSessionId, out var assessmentSessionIdValue)
            ? new ConnectAppraiserCommand(assessmentSessionIdValue, userId, userName)
            : null;
    }

    private async Task<IBaseRequest?> FindDraftSession(
        string commandText,
        long userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        
        using var scope = _serviceProvider.CreateScope();
        var assessmentSessionRepository = scope.ServiceProvider.GetRequiredService<IAssessmentSessionRepository>();

        var assessmentSession = await assessmentSessionRepository.FindByModerator(new AppraiserId(userId), cancellationToken);

        return assessmentSession?.State == AssessmentSessionState.Draft
            ? new ActivateAssessmentSessionCommand(userId, commandText)
            : null;
    }
}