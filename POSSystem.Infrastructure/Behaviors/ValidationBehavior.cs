using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Exceptions;
using POSSystem.Domain.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace POSSystem.Infrastructure.Behaviors;

/// <summary>
/// Validation behavior for MediatR pipeline.
/// Follows Single Responsibility and Cross-cutting Concerns principles.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<FluentValidation.IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<FluentValidation.IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new FluentValidation.ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            throw new FluentValidation.ValidationException(failures);
        }

        return await next();
    }
}


