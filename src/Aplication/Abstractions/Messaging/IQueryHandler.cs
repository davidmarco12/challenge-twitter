namespace TwitterAPI.Application.Abstractions.Messaging
{
    using TwitterAPI.Interfaces.Responses;
    using MediatR;

    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IResponse<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
