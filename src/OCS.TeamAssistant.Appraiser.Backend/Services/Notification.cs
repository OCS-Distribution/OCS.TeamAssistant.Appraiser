namespace OCS.TeamAssistant.Appraiser.Backend.Services;

public sealed record Notification(string Message, long[] UserIds)
{
    public static readonly Notification Empty = new Notification(string.Empty, Array.Empty<long>());
}