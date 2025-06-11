namespace TwitterAPI.Application.Abstractions.Messaging
{
    using TwitterAPI.Interfaces.Responses;
    using MediatR;

    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, IResponse>
        where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, IResponse<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}
