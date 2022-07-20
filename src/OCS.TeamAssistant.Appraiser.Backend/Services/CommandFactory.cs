using System.Text;
using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ActivateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.AddTask;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ConnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.DisconnectAppraiser;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndAssessmentSession;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EndEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.EstimateStory;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ResetEstimate;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.Restart;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.ShowAppraiserList;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class CommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly Dictionary<string, Func<string, long, long, string, IBaseRequest?>> _commandList;
    private readonly IReadOnlyCollection<(string Command, MessageId MessageId)> _commandHelp;
    private readonly HashSet<string> _targets;

    public CommandFactory(IServiceProvider serviceProvider, IMessageBuilder messageBuilder)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));

        _commandHelp = new []
        {
            (Commands.Disconnect, MessageId.DisconnectCommandHelp),
            (Commands.New, MessageId.NewCommandHelp),
            (Commands.Users, MessageId.UsersCommandHelp),
            (Commands.Add, MessageId.AddCommandHelp),
            (Commands.Set, MessageId.SetCommandHelp),
            (Commands.NoIdea, MessageId.NoIdeaCommandHelp),
            (Commands.End, MessageId.EndCommandHelp),
            (Commands.Reset, MessageId.ResetCommandHelp),
            (Commands.Exit, MessageId.ExitCommandHelp)
        };
        
        _commandList = new()
        {
            [Commands.Start] = (c, cId, uId, uName) => CreateConnectAppraiserCommand(c, uId, uName),
            [Commands.Disconnect] = (c, cId, uId, uName) => new DisconnectAppraiserCommand(uId, uName),
            [Commands.New] = (c, cId, uId, uName) => new CreateAssessmentSessionCommand(cId, uId, uName),
            [Commands.Users] = (c, cId, uId, uName) => new ShowAppraiserListCommand(uId, uName),
            [Commands.Add] = (c, cId, uId, uName) => CreateAddStoryCommand(c, uId, uName),
            [Commands.Set] = (c, cId, uId, uName) => CreateEstimateStoryCommand(c, uId, uName),
            [Commands.NoIdea] = (c, cId, uId, uName) => new EstimateStoryCommand(uId, uName, Value: null),
            [Commands.End] = (c, cId, uId, uName) => new EndEstimateCommand(uId, uName),
            [Commands.Reset] = (c, cId, uId, uName) => new ResetEstimateCommand(uId, uName),
            [Commands.Exit] = (c, cId, uId, uName) => new EndAssessmentSessionCommand(uId, uName),
            [Commands.Restart] = (c, cId, uId, uName) => new RestartCommand(),
            [Commands.Help] = (c, cId, uId, uName) => CreateHelpCommand()
        };
        
        _targets = new HashSet<string>(AssessmentValueRules.GetAssessments.Select(a => ((int)a).ToString()));
    }

    public async Task<IBaseRequest?> Create(
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

        var commandKey = _commandList.Keys
            .FirstOrDefault(k => commandText.StartsWith(k, StringComparison.InvariantCultureIgnoreCase));
        
        if (commandKey is not null)
            command = _commandList[commandKey](commandText, chatId, userId, userName);

        command ??= await FindDraftSession(commandText, userId, userName, cancellationToken);

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

    private SendMessageCommand CreateHelpCommand()
    {
        var messageBuilder = new StringBuilder();

        foreach (var item in _commandHelp)
            messageBuilder.AppendLine(_messageBuilder.Build(item.MessageId, item.Command));

        return new SendMessageCommand(messageBuilder.ToString());
    }

    private IBaseRequest CreateEstimateStoryCommand(string commandText, long userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

        const int estimateIndex = 3;
        var parameter = commandText.Substring(estimateIndex, commandText.Length - estimateIndex);

        return _targets.Contains(parameter)
            ? new EstimateStoryCommand(userId, userName, int.Parse(parameter))
            : new SendMessageCommand(_messageBuilder.Build(
                MessageId.EnterEstimateWithError,
                parameter,
                string.Join(", ", _targets)));
    }

    private IBaseRequest CreateConnectAppraiserCommand(string commandText, long userId, string userName)
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
            : new SendMessageCommand(_messageBuilder.Build(MessageId.EnterCommandWithError));
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

        var assessmentSession = await assessmentSessionRepository.Find(
            new AppraiserId(userId),
            cancellationToken);

        return assessmentSession?.State == AssessmentSessionState.Draft
            ? new ActivateAssessmentSessionCommand(userId, userName, commandText)
            : null;
    }
}