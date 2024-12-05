using ChatService.Application.Features.Common.UserRequest;
using MediatR;
using System.Security.Claims;

namespace ChatService.Extensions
{
    public static class MediatrExtensions
    {

        public static Task<TResponse> SendHttpContext<TRequest, TResponse>(this IMediator mediator, HttpContext httpContext, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
            => mediator.Send(new UserRequest<TRequest, TResponse>(httpContext.User.GetUserId(), request), cancellationToken);

        public static Task SendHttpContext<TRequest>(this IMediator mediator, HttpContext httpContext, TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
            => mediator.Send(new UserRequest<TRequest>(httpContext.User.GetUserId(), request), cancellationToken);
    }
}
