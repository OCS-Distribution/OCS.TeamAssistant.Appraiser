namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface INotificationBuilder<in TCommandResult>
{
	IEnumerable<INotificationMessage> Build(TCommandResult commandResult, long fromId);
}