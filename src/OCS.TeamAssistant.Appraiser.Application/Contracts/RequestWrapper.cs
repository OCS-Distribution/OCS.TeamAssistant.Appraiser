using MediatR;

namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public sealed record RequestWrapper<TRequest, TResponse>(TRequest Request) : IRequest<TResponse>;