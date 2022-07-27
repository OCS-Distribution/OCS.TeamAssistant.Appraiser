using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.Connect;

public interface IConnectCommand : IRequest<ConnectResult>
{
	Guid AssessmentSessionId { get; }
	long AppraiserId { get; }
	string AppraiserName { get; }
}