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
            [Commands.Start] = (CreateConnectAppraiserCommand, string.Empty),
            [Commands.New] = ((c, uId, uName) => new CreateAssessmentSessionCommand(uId, uName), $"{Commands.New} - создать сессию"),
            [Commands.Users] = ((c, uId, uName) => new ShowAppraiserListCommand(uId, uName), $"{Commands.Users} - список пользователей"),
            [Commands.Add] = (CreateAddStoryCommand, $"{Commands.Add}{{task}} - добавление задачи для оценки"),
            [Commands.Set] = (CreateEstimateStoryCommand, $"{Commands.Set}{{3}} - задать оценку"),
            [Commands.End] = ((c, uId, uName) => new EndEstimateCommand(uId, uName), $"{Commands.End} - завершение оценки"),
            [Commands.Exit] = ((c, uId, uName) => new EndAssessmentSessionCommand(uId, uName), $"{Commands.Exit} - завершение сессии"),
            [Commands.Help] = ((c, uId, uName) => CreateHelpCommand(), string.Empty)
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

        command ??= await FindDraftSession(commandText, userId, userName, cancellationToken);
        command ??= CreateErrorHandleCommand();
        
        return command;
    }

    private AddStoryCommand CreateAddStoryCommand(string commandText, long userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        
        var title = commandText.Replace(Commands.Add, string.Empty);

        return new AddStoryCommand(userId, userName, title);
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

    private IBaseRequest CreateEstimateStoryCommand(string commandText, long userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

        const int estimateIndex = 4;

        if (commandText.Length <= estimateIndex)
            return new EstimateStoryCommand(userId, userName, Value: null);

        var parameter = commandText.Substring(estimateIndex, commandText.Length - estimateIndex);

        return _targets.Contains(parameter)
            ? new EstimateStoryCommand(userId, userName, int.Parse(parameter))
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
        string userName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        
        using var scope = _serviceProvider.CreateScope();
        var assessmentSessionRepository = scope.ServiceProvider.GetRequiredService<IAssessmentSessionRepository>();

        var assessmentSession = await assessmentSessionRepository.FindByModerator(new AppraiserId(userId), cancellationToken);

        return assessmentSession?.State == AssessmentSessionState.Draft
            ? new ActivateAssessmentSessionCommand(userId, userName, commandText)
            : null;
    }
}