using MassTransit;
using MediatR;

namespace ChatService.Application.Features.Common.UserRequest
{
    public record UserRequest<TRequest>(string UserId, TRequest Params) : IRequest;

    public record UserRequest<TRequest, TResponse>(string UserId, TRequest Params): IRequest<TResponse>;
}
