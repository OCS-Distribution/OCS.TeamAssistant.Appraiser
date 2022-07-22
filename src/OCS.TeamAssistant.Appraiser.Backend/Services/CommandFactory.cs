using System.Text;
using MediatR;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.SendMessage;
using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Backend.Commands;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.AssessmentValues;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class CommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly Dictionary<string, Func<string, long, long, string, IBaseRequest?>> _commandList;
    private readonly IReadOnlyCollection<(string Command, MessageId MessageId)> _staticHelp;
    private readonly HashSet<string> _targets;

    public CommandFactory(IServiceProvider serviceProvider, IMessageBuilder messageBuilder)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));

        _staticHelp = new []
        {
            (CommandsList.New, MessageId.NewCommandHelp),
            (CommandsList.Add, MessageId.AddCommandHelp),
            (CommandsList.Reset, MessageId.ResetCommandHelp),
            (CommandsList.End, MessageId.EndCommandHelp),
            (CommandsList.Exit, MessageId.ExitCommandHelp),
            (CommandsList.Users, MessageId.UsersCommandHelp),
            (CommandsList.Disconnect, MessageId.DisconnectCommandHelp),
            (CommandsList.NoIdea, MessageId.NoIdeaCommandHelp)
        };

        _commandList = new()
        {
            [CommandsList.Start] = (c, cId, uId, uName) => CreateConnectAppraiserCommand(c, uId, uName),
            [CommandsList.Disconnect] = (c, cId, uId, uName) => new DisconnectAppraiserCommand(uId, uName),
            [CommandsList.New] = (c, cId, uId, uName) => new CreateAssessmentSessionCommand(cId, uId, uName),
            [CommandsList.Users] = (c, cId, uId, uName) => new ShowAppraiserListQuery(uId, uName),
            [CommandsList.Add] = (c, cId, uId, uName) => CreateStartStorySelectionCommand(uId, uName),
            [CommandsList.Set] = (c, cId, uId, uName) => CreateEstimateStoryCommand(c, uId, uName),
            [CommandsList.NoIdea] = (c, cId, uId, uName) => new EstimateStoryCommand(uId, uName, Value: null),
            [CommandsList.End] = (c, cId, uId, uName) => new EndEstimateCommand(uId, uName),
            [CommandsList.Reset] = (c, cId, uId, uName) => new ResetEstimateCommand(uId, uName),
            [CommandsList.Exit] = (c, cId, uId, uName) => new EndAssessmentSessionCommand(uId, uName),
            [CommandsList.Help] = (c, cId, uId, uName) => CreateHelpCommand()
        };

        _targets = new(AssessmentValueRules.GetAssessments.Select(a => ((int)a).ToString()));
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

        command ??= await FindCommandBySession(commandText, userId, userName, cancellationToken);

        return command;
    }

    private IStartStorySelectionCommand CreateStartStorySelectionCommand(long userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

        return new StartStorySelectionCommand(userId, userName);
    }

    private ISendMessageCommand CreateHelpCommand()
    {
        var messageBuilder = new StringBuilder();

        foreach (var item in _staticHelp)
            messageBuilder.AppendLine(_messageBuilder.Build(item.MessageId, item.Command));

        foreach (var item in AssessmentValueRules.GetAssessments)
            messageBuilder.AppendLine(_messageBuilder.Build(MessageId.SetCommandHelp, CommandsList.Set, (int)item));

        return new SendMessageCommand(messageBuilder.ToString());
    }

    private IBaseRequest CreateEstimateStoryCommand(string commandText, long userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(commandText));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

        var parameter = new string(commandText.Where(Char.IsDigit).ToArray());

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

        var assessmentSessionId = commandText.Replace(CommandsList.Start, string.Empty);

        return !string.IsNullOrWhiteSpace(userName) && Guid.TryParse(assessmentSessionId, out var value)
            ? new ConnectAppraiserCommand(value, userId, userName)
            : new SendMessageCommand(_messageBuilder.Build(MessageId.EnterCommandWithError));
    }

    private async Task<IBaseRequest?> FindCommandBySession(
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

        var assessmentSession = await assessmentSessionRepository.Find(new AppraiserId(userId), cancellationToken);

        return assessmentSession?.State switch
        {
            AssessmentSessionState.Draft => new ActivateAssessmentSessionCommand(userId, userName, commandText),
            AssessmentSessionState.StorySelection => new AddStoryCommand(userId, userName, commandText),
            _ => null
        };
    }
}