namespace TwitterAPI.Application.Abstractions.Messaging
{
    using TwitterAPI.Interfaces.Responses;
    using MediatR;

    public interface IQuery<TResponse> : IRequest<IResponse<TResponse>>
    {
    }
}
