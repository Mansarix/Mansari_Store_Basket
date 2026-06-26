using Mansari.Store.Basket.Application.Common.Dispatchers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mansari.Store.Basket.Application.Common.Behaviors;

namespace Mansari.Store.Basket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        //services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}
