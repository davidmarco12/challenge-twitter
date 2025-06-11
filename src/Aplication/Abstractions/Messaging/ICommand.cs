namespace TwitterAPI.Application.Abstractions.Messaging
{
    using TwitterAPI.Interfaces.Responses;
    using MediatR;

    public interface ICommand : IRequest<IResponse>, IBaseCommand
    {
    }

    public interface ICommand<TResponse> : IRequest<IResponse<TResponse>>, IBaseCommand
    {
    }

    public interface IBaseCommand
    {
    }
}
