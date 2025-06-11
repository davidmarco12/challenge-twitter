namespace TwitterAPI.Application.Abstractions.Messaging
{
    using TwitterAPI.Interfaces.Responses;
    using MediatR;

    public interface IQueryPaginatedHandler<TQuery, TResponse> : IRequestHandler<TQuery, IPaginatedResponse<TResponse>>
        where TQuery : IQueryPaginated<TResponse>
    {
    }
}
