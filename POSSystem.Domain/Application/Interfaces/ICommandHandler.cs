using MediatR;

namespace POSSystem.Domain.Application.Interfaces;

/// <summary>
/// CQRS Command Handler interface following Dependency Inversion Principle.
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
}

/// <summary>
/// CQRS Command Handler for commands that don't return a value.
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : IRequest
{
}


