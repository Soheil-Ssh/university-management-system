using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions.Behaviors;

namespace SharedKernel.Abstractions;

// ReSharper disable once ConvertToExtensionBlock
public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelAbstractions<TMarker>(this IServiceCollection services)
        => services
            .AddSharedMediatR<TMarker>()
            .AddSharedFluentValidation<TMarker>()
            .AddSharedPipelineBehaviors();

    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddSharedMediatR<TMarker>(
        this IServiceCollection services)
        => services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<TMarker>());

    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddSharedFluentValidation<TMarker>(this IServiceCollection services)
        => services.AddValidatorsFromAssemblyContaining<TMarker>();

    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddSharedPipelineBehaviors(this IServiceCollection services)
        => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
}