using FluentValidation;
using Mansari.Store.Basket.Application.Common.Behaviors;
using Mansari.Store.Basket.Application.Common.Dispatchers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Mansari.Store.Basket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(assembly);

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}
