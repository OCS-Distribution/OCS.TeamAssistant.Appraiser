using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.DisconnectAppraiser;

public interface IDisconnectAppraiserCommand : IRequest<DisconnectAppraiserResult>
{
	long AppraiserId { get; }
	string AppraiserName { get; }
}