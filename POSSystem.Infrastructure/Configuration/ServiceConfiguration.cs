using POSSystem.Domain.Application.Mappings;
using POSSystem.Infrastructure.Behaviors;
using POSSystem.Infrastructure.Middleware;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace POSSystem.Infrastructure.Configuration;

/// <summary>
/// Centralized service configuration following DRY and composition patterns.
/// Provides a fluent API for consistent microservice setup.
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Configures all common microservice dependencies.
    /// </summary>
    public static IServiceCollection AddPosServiceDefaults(
        this IServiceCollection services,
        params Type[] validatorTypes)
    {
        // AutoMapper for DTO mapping
        services.AddAutoMapper(typeof(MappingProfile));

        // Get assemblies from types
        var assemblies = validatorTypes
            .Select(t => t.Assembly)
            .Distinct()
            .ToArray();

        // MediatR for CQRS pattern - register with assemblies
        services.AddMediatR(assemblies);

        // Register behaviors manually for MediatR 11.x
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // FluentValidation
        services.AddValidatorsFromAssemblies(assemblies);

        // Problem details for API errors
        services.AddProblemDetails();

        return services;
    }

    /// <summary>
    /// Configures the application middleware pipeline.
    /// </summary>
    public static WebApplication UsePosApplicationPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
        }

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        return app;
    }
}


