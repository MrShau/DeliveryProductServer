using Microsoft.EntityFrameworkCore;

namespace DeliveryProductAPI.Server.Services
{
    public class DeliveryStatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(3);

        public DeliveryStatusUpdater(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateDeliveryStatuses();
                await Task.Delay(_updateInterval, stoppingToken);
            }
        }

        private async Task UpdateDeliveryStatuses()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pendingDeliveries = await dbContext.Orders
                .Where(d => d.StatusId < 4 && d.StatusId > 1)
                .ToListAsync();

            foreach (var delivery in pendingDeliveries)
            {
                delivery.StatusId++;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
