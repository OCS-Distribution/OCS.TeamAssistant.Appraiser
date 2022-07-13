using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.SendMessage;

public sealed record SendMessageCommand(string Text) : IRequest<SendMessageResult>;