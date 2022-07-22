namespace OCS.TeamAssistant.Appraiser.Domain.Keys;

public sealed record MessageId
{
    public string Value { get; }

    public MessageId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

        Value = value;
    }
    
    public static readonly MessageId NoRightsAddTaskToSession = new(nameof(NoRightsAddTaskToSession));
    public static readonly MessageId ShutdownCompletedWithError = new(nameof(ShutdownCompletedWithError));
    public static readonly MessageId ModeratorCannotDisconnectedFromSession = new(nameof(ModeratorCannotDisconnectedFromSession));
    public static readonly MessageId MissingTaskForEvaluate = new(nameof(MissingTaskForEvaluate));
    
    public static readonly MessageId SessionNotFoundForModerator = new(nameof(SessionNotFoundForModerator));
    public static readonly MessageId SessionNotFoundForAppraiser = new(nameof(SessionNotFoundForAppraiser));
    public static readonly MessageId AppraiserConnectWithError = new(nameof(AppraiserConnectWithError));
    public static readonly MessageId AppraiserConnectedToOtherSession = new(nameof(AppraiserConnectedToOtherSession));
    public static readonly MessageId SessionExistsForModerator = new(nameof(SessionExistsForModerator));
    public static readonly MessageId EstimateRejected = new(nameof(EstimateRejected));
    
    public static readonly MessageId ActiveSessionsFound = new(nameof(ActiveSessionsFound));

    public static readonly MessageId EnterSessionName = new(nameof(EnterSessionName));
    public static readonly MessageId EnterStoryName = new(nameof(EnterStoryName));
    public static readonly MessageId ConnectToSession = new(nameof(ConnectToSession));
    public static readonly MessageId ConnectedSuccess = new(nameof(ConnectedSuccess));
    public static readonly MessageId AppraiserAdded = new(nameof(AppraiserAdded));
    public static readonly MessageId DisconnectedFromSession = new(nameof(DisconnectedFromSession));
    public static readonly MessageId AppraiserDisconnectedFromSession = new(nameof(AppraiserDisconnectedFromSession));
    public static readonly MessageId SessionEnded = new(nameof(SessionEnded));
    public static readonly MessageId EstimateRepeated = new(nameof(EstimateRepeated));
    public static readonly MessageId EndEstimate = new(nameof(EndEstimate));
    public static readonly MessageId NeedEstimate = new(nameof(NeedEstimate));
    public static readonly MessageId TotalEstimate = new(nameof(TotalEstimate));
    public static readonly MessageId EnterEstimate = new(nameof(EnterEstimate));
    public static readonly MessageId AppraiserList = new(nameof(AppraiserList));

    public static readonly MessageId EnterCommandWithError = new(nameof(EnterCommandWithError));
    public static readonly MessageId EnterEstimateWithError = new(nameof(EnterEstimateWithError));
    public static readonly MessageId DisconnectCommandHelp = new(nameof(DisconnectCommandHelp));
    public static readonly MessageId NewCommandHelp = new(nameof(NewCommandHelp));
    public static readonly MessageId UsersCommandHelp = new(nameof(UsersCommandHelp));
    public static readonly MessageId AddCommandHelp = new(nameof(AddCommandHelp));
    public static readonly MessageId SetCommandHelp = new(nameof(SetCommandHelp));
    public static readonly MessageId NoIdeaCommandHelp = new(nameof(NoIdeaCommandHelp));
    public static readonly MessageId EndCommandHelp = new(nameof(EndCommandHelp));
    public static readonly MessageId ResetCommandHelp = new(nameof(ResetCommandHelp));
    public static readonly MessageId ExitCommandHelp = new(nameof(ExitCommandHelp));
}