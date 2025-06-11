namespace TwitterAPI.Application.Abstractions.Messaging
{
    using TwitterAPI.Interfaces.Responses;
    using MediatR;

    public interface IQueryPaginated<TResponse> : IRequest<IPaginatedResponse<TResponse>>
    {
    }
}
