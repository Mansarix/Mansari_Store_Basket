using Mansari.Store.Basket.Application.Common.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mansari.Store.Basket.Infrastructure.BackgroundServices;

public sealed class BasketExpirationBackgroundService(
    IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                await dispatcher.SendAsync(new ExpireBasketsCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
