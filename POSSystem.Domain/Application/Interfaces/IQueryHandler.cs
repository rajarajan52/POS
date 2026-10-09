using MediatR;

namespace POSSystem.Domain.Application.Interfaces;

/// <summary>
/// CQRS Query Handler interface following Dependency Inversion Principle.
/// </summary>
/// <typeparam name="TQuery">The query type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IRequest<TResponse>
{
}


