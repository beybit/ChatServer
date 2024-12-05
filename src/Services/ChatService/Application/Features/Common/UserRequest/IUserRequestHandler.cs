using MediatR;

namespace ChatService.Application.Features.Common.UserRequest
{
    public interface IUserRequestHandler<TRequest, TResponse>
        : IRequestHandler<UserRequest<TRequest, TResponse>, TResponse>
        where TResponse : class
        where TRequest : class
    {

    }
    public interface IUserRequestHandler<TRequest>
        : IRequestHandler<UserRequest<TRequest>>
        where TRequest : class
    {

    }
}
