using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Disconnect;

public interface IDisconnectCommand : IRequest<DisconnectResult>
{
	long AppraiserId { get; }
	string AppraiserName { get; }
}