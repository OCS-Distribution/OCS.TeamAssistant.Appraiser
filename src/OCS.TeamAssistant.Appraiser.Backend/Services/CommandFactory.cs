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
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class CommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, (Func<string, long, long, string, IBaseRequest?> Func, string Help)> _commands;
    private readonly HashSet<string> _targets;

    public CommandFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        
        _commands = new()
        {
            [Commands.Start] = ((c, cId, uId, uName) => CreateConnectAppraiserCommand(c, uId, uName), string.Empty),
            [Commands.New] = (
                (c, cId, uId, uName) => new CreateAssessmentSessionCommand(cId, uId, uName),
                $"{Commands.New} - создать сессию"),
            [Commands.Users] = (
                (c, cId, uId, uName) => new ShowAppraiserListCommand(uId, uName),
                $"{Commands.Users} - список пользователей"),
            [Commands.Add] = (
                (c, cId, uId, uName) => CreateAddStoryCommand(c, uId, uName),
                $"{Commands.Add}{{task}} - добавление задачи для оценки"),
            [Commands.Set] = (
                (c, cId, uId, uName) => CreateEstimateStoryCommand(c, uId, uName),
                $"{Commands.Set}{{3}} - задать оценку"),
            [Commands.End] = (
                (c, cId, uId, uName) => new EndEstimateCommand(uId, uName),
                $"{Commands.End} - завершение оценки"),
            [Commands.Exit] = (
                (c, cId, uId, uName) => new EndAssessmentSessionCommand(uId, uName),
                $"{Commands.Exit} - завершение сессии"),
            [Commands.Help] = ((c, cId, uId, uName) => CreateHelpCommand(), string.Empty)
        };
        
        var values = Enum.GetValues<AssessmentValue>()
            .Where(i => i > 0)
            .Select(i => ((int)i).ToString());
        _targets = new HashSet<string>(values);
    }

    public async Task<IBaseRequest> Create(
        string commandText,
        long chatId,
        long userId,
        string userName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
        
        IBaseRequest? command = null;

        var commandKey = _commands.Keys.FirstOrDefault(commandText.StartsWith);
        if (commandKey is not null)
            command = _commands[commandKey].Func(commandText, chatId, userId, userName);

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

        foreach (var command in _commands.Values)
            if (!string.IsNullOrWhiteSpace(command.Help))
                messageBuilder.AppendLine(command.Help);

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
        
        const int parameterIndex = 1;
        var commands = commandText.Split(' ');
        var assessmentSessionId = commands.Length > parameterIndex ? commands[parameterIndex] : string.Empty;

        return !string.IsNullOrWhiteSpace(userName) && Guid.TryParse(assessmentSessionId, out var value)
            ? new ConnectAppraiserCommand(value, userId, userName)
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

        var assessmentSession = await assessmentSessionRepository.FindByModerator(
            new AppraiserId(userId),
            cancellationToken);

        return assessmentSession?.State == AssessmentSessionState.Draft
            ? new ActivateAssessmentSessionCommand(userId, userName, commandText)
            : null;
    }
}