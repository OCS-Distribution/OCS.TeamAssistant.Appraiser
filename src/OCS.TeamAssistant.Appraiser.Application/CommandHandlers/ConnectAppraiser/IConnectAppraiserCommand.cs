using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectAppraiser;

public interface IConnectAppraiserCommand : IRequest<ConnectAppraiserResult>
{
	Guid AssessmentSessionId { get; }
	long AppraiserId { get; }
	string AppraiserName { get; }
}